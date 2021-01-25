using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationHelper.Http
{
    public class HttpHelper : IHttpHelper
    {
        public async Task<HttpResponseMessage> GetHttpResponse<T>(string url, string method, T body = null) where T : class
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

            StringContent httpBody = null;

            if (body != null)
                httpBody = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            switch (method)
            {
                case "Post":
                    return await httpClient.PostAsync(url, httpBody);
                case "Put":
                    return await httpClient.PutAsync(url, httpBody);
                case "Get":
                    return await httpClient.GetAsync(url);
                case "Delete":
                    return await httpClient.DeleteAsync(url);
                default: return null;
            }
        }
    }
}
