using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.Commitments.Api.Client
{
    internal class HttpClientHelper 
    {
        private readonly HttpClient _client;
        private readonly QueryStringHelperTemp _queryStringHelper;

        internal HttpClientHelper(HttpClient client)
        {
            _client = client;
            _queryStringHelper = new QueryStringHelperTemp();
        }

        public async Task<T> GetAsync<T>(string url)
        {
            return await GetAsync<T>(url, null);
        }


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
                    string.Format("{0}{1}", (object) url, (object) this._queryStringHelper.GetQueryString(data))));
            }

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task PatchAsync<T>(string url, T data)
        {
            string json = JsonConvert.SerializeObject(data);

            HttpResponseMessage response = await this._client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = (HttpContent)new StringContent(json, Encoding.UTF8, "application/json")
            });

            if (!response.IsSuccessStatusCode)
            {
                await HandleException(response);
            }
        }

        public async Task PostAsync<T>(string url, T data)
        {
            await PostAsync(url, data, HttpMethod.Get);
        }

        public async Task PostAsync<T>(string url, T data, HttpMethod method)
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

        private async Task HandleException(HttpResponseMessage response)
        {

            var content = await response.Content.ReadAsStringAsync();

            JObject jObj = JObject.Parse(content);
            if (jObj.HasValues)
            {
                var errorReponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                throw new ApiFaultException(response.StatusCode, errorReponse);
            }

            if (response.Content != null)
                response.Content.Dispose();

            throw new HttpRequestException($"Invalid Http Request Status Code {response.StatusCode}");

        }
    }


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
