using CrossCutting.Cryptography.Entities;
using CrossCutting.Cryptography.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace CrossCutting.ApiHandler
{

    public static class ParametersHelper {

        /// <summary>
        /// Convert a list of parameters to String
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>A string</returns>
        public static string Generate(this List<Parameters> parameters) {

            return string.Join("&", parameters.Select(f => f.ToString()));
        }

        public static FormUrlEncodedContent GenerateFormContent(this List<Parameters> formUrlEncodedParameters)
        {
            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formUrlEncodedParameters.ForEach(f => list.Add(new KeyValuePair<string, string>(f.Name, f.Value.ToString())));

            return new FormUrlEncodedContent(list.ToArray());
        }

        public static List<KeyValuePair<string,string>> ListKeyValues(this List<Parameters> formUrlEncodedParameters)
        {
            var listKeyValues = new List<KeyValuePair<string, string>>();
            listKeyValues.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formUrlEncodedParameters.ForEach(f => listKeyValues.Add(new KeyValuePair<string, string>(f.Name, f.Value.ToString())));

            return listKeyValues;
        }

        public static string GenerateEncrypted(this List<Parameters> parameters, string key) {
            ICryptography crypt = new AES(parameters.ToList().Generate(), key);
            return crypt.Encrypt();
        }

    }

    public class Parameters {

        public string Name { get; private set; }
        public object Value { get; private set; }

        public Parameters(Expression<Func<object>> obj) {
            var member = (MemberExpression)obj.Body;
            Name = member.Member.Name;
            Value = obj;
        }

        public Parameters(string name, object value) {
            Name = name;
            Value = value;
        }

        public override string ToString() {
            return string.Format("{0}={1}", Name, Value);
        }

    }
}
