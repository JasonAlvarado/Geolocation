using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHelper.Http
{
    public interface IHttpHelper
    {
        Task<HttpResponseMessage> Get(string url);
        Task<HttpResponseMessage> Post<T>(string url, T body) where T : class;
        Task<HttpResponseMessage> Put<T>(string url, T body) where T : class;
        Task<HttpResponseMessage> Delete(string url);
    }
}
