using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.Core.Common
{
    public class HttpClientHelper 
    {
        private readonly HttpClient _client;
        private readonly QueryStringHelperTemp _queryStringHelper;

        public HttpClientHelper(HttpClient client)
        {
            _client = client;
            _queryStringHelper = new QueryStringHelperTemp();
        }

        public Task<T> GetAsync<T>(string url) => GetAsync<T>(url, null);

        public async Task<T> GetAsync<T>(string url, object data)
        {
            HttpResponseMessage response;

            if (data == null)
            {
                response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            }
            else
            {
                response = await this._client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                    $"{url}{_queryStringHelper.GetQueryString(data)}"));
            }

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public Task PatchAsync<T>(string url, T data) => PostAsync(url, data, new HttpMethod("PATCH"));
        public Task PostAsync<T>(string url, T data) => PostAsync(url, data, HttpMethod.Post);
        public Task DeleteAsync(string url) => PostAsync(url, HttpMethod.Delete);
        public Task DeleteAsync<T>(string url, T data) => PostAsync(url, data, HttpMethod.Delete);
        public Task PutAsync<T>(string url, T data) => PostAsync(url, data, HttpMethod.Put);
        public Task<TOutput> PostAsync<TInput, TOutput>(string url, TInput data) => PostAsync<TInput, TOutput>(url, data, HttpMethod.Post);

        private async Task<TOutput> PostAsync<TInput, TOutput>(string url, TInput data, HttpMethod method)
        {
            string json = JsonConvert.SerializeObject(data);

            HttpResponseMessage response = await this._client.SendAsync(new HttpRequestMessage(method, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }

            return JsonConvert.DeserializeObject<TOutput>(await response.Content.ReadAsStringAsync());
        }

        private async Task PostAsync<T>(string url, T data, HttpMethod method)
        {
            string json = JsonConvert.SerializeObject(data);

            HttpResponseMessage response = await this._client.SendAsync(new HttpRequestMessage(method, url)
            {
                Content = (HttpContent)new StringContent(json, Encoding.UTF8, "application/json")
            });

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }
        }

        private async Task PostAsync(string url, HttpMethod method)
        {
            HttpResponseMessage response = await this._client.SendAsync(new HttpRequestMessage(method, url));

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }
        }


        private async Task HandleException(HttpResponseMessage response)
        {

            var content = await response.Content.ReadAsStringAsync();

            JObject jObj = JObject.Parse(content);
            if (jObj.HasValues)
            {
                var errorReponses = JsonConvert.DeserializeObject<ErrorResponses>(content);
                throw new ApiFaultException(response.StatusCode, errorReponses);
            }

            if (response.Content != null)
                response.Content.Dispose();

            throw new HttpRequestException($"Invalid Http Request Status Code {response.StatusCode}");

        }
    }

    // This is to be replaced by real helper class
    internal sealed class QueryStringHelperTemp
    {
        public string GetQueryString(object obj)
        {
            if (obj == null)
                return string.Empty;
            List<string> source1 = new List<string>();
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>)obj.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => p.GetValue(obj, (object[])null) != null)))
            {
                PropertyInfo p = propertyInfo;
                object obj1 = p.GetValue(obj, (object[])null);
                ICollection source2 = obj1 as ICollection;
                if (source2 != null)
                    source1.AddRange(source2.Cast<object>().Select<object, string>((Func<object, string>)(v => string.Format("{0}={1}", (object)p.Name, v))));
                else
                    source1.Add(string.Format("{0}={1}", (object)p.Name, obj1));
            }
            if (!source1.Any<string>())
                return string.Empty;
            return "?" + string.Join("&", source1.ToArray());
        }
    }

}
