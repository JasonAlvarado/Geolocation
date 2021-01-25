using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHelper.Http
{
    public interface IHttpHelper
    {
        Task<HttpResponseMessage> GetHttpResponse<T>(string url, string method, T body = null) where T : class;
    }
}
