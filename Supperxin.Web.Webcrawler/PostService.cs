using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
namespace Supperxin.Web.Webcrawler
{

    class PostService
    {
        private static HttpClient httpClient;
        static PostService()
        {
            httpClient = new HttpClient();

        }
        public static bool PostData(object data, string url, bool doserialize = true)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = doserialize ? JsonConvert.SerializeObject(data) : data.ToString();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

            return result.IsSuccessStatusCode;
        }
    }
}