using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.ApiHandler.Interfaces
{
    public interface IHandler
    {
        string GetAsString(string url);
        string GetAsString(string url, List<Parameters> parameters);
        string Post<T>(string url, T obj);
        string Post<T>(string url, List<Parameters> parameters, T obj);
    }
}
