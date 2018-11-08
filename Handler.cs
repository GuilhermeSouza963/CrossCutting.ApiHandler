using CrossCutting.ApiHandler.Interfaces;
using CrossCutting.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.ApiHandler
{
    public class Handler : IHandler
    {
        private readonly string _baseUrl;

        private string _url;
        public string UrlAuth { get; }
        public List<Parameters> Parameters { get; set; }
        public List<Parameters> AuthorizationParams { get; }

        public List<KeyValuePair<string, string>> listKeyValues;
        public bool HasAuthorization
        {
            get { return AuthorizationParams.Any(); }
        }

        public string Url {
            get {
                return Parameters.Any()
                    ? string.Format("{0}?{1}", UrlUtil.Combine(_baseUrl, _url), Parameters.Generate())
                    : UrlUtil.Combine(_baseUrl, _url);
            }
            private set {
                _url = value;
                Uri = new Uri(_baseUrl);
            }
        }

        public Uri Uri { get; private set; }

        public Handler() {
            Parameters = new List<Parameters>();
            AuthorizationParams = new List<Parameters>();
        }

        public Handler(string baseUrl) : this() {
            _baseUrl = baseUrl;
            UrlAuth = string.Format("{0}{1}", baseUrl, "/oauth2/token");
        }

        public Handler(string baseUrl, List<Parameters> authorizationParams) : this(baseUrl) {
            AuthorizationParams = authorizationParams;
            listKeyValues = AuthorizationParams.ListKeyValues();
        }
        
        private HttpClient GenerateClient()
        {
            var client = new HttpClient { BaseAddress = Uri };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<HttpResponseMessage> GetAsync() {
            using (var client = GenerateClient()) {
                if (HasAuthorization)
                {
                    string access_token = PostOAuth();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                }

                return await client.GetAsync(Url);
            }
        }

        public HttpResponseMessage Get() {
            return Task.Run(GetAsync).Result;
        }

        public string GetAsString(string url) {
            Url = url;
            return Get().Content.ReadAsStringAsync().Result;
        }
        public string GetAsString(string url , List<Parameters> parameters)
        {
            Parameters = parameters;
            return GetAsString(url);
        }

        public async Task<MemoryStream> GetAsStream(string fileUrl)
        {
            using (var client = new HttpClient())
            {
                byte[] data = null;
                var task = Task.Run(async () => { data = await client.GetByteArrayAsync(fileUrl); });
                task.Wait();
                MemoryStream memoryStream = new MemoryStream(data);
                return memoryStream;

            }
        }

        public string Post<T>(string url, T obj) {
            Url = url;
            using (var client = GenerateClient()) {
                if (HasAuthorization)
                {
                    string access_token = PostOAuth();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                }
                using (HttpResponseMessage response = client.PostAsync(Url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).Result) {
                    using (HttpContent content = response.Content) {
                        return content.ReadAsStringAsync().Result;
                    }
                }
            }
        }

        public string Post<T>(string url, List<Parameters> parameters, T obj)
        {
            Parameters = parameters;
            return Post<T>(url,obj);
        }

        private string PostOAuth()
        {
            using (var client = GenerateClient())
            {
                using (HttpResponseMessage response = client.PostAsync(UrlAuth, new FormUrlEncodedContent(listKeyValues)).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return JObject.Parse(content.ReadAsStringAsync().Result).GetValue("access_token").ToString();
                    }
                }
            }
        }
    }
}
