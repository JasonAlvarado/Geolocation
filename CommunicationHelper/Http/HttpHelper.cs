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
        private readonly HttpClient httpClient;

        public HttpHelper()
        {
            httpClient = new HttpClient();

            // mozilla firefox http headers
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            return await httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> Post<T>(string url, T body) where T : class
        {
            return await httpClient.PostAsync(url, CreateHttpBody(body));
        }

        public async Task<HttpResponseMessage> Put<T>(string url, T body) where T : class
        {
            return await httpClient.PutAsync(url, CreateHttpBody(body));
        }

        public async Task<HttpResponseMessage> Delete(string url)
        {
            return await httpClient.DeleteAsync(url);
        }

        private StringContent CreateHttpBody<T>(T body) where T : class
        {
            return new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        }
    }
}
