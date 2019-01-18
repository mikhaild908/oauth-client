using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace oauth_client
{
    class Program
    {
        const string CLIENT_ID = "<client id>";
        const string CLIENT_SECRET = "<client secret>";
        const string API_KEY = "<api key>";
        const string GET_URL = "https://<hostname>/v1/referlink";
        const string ACCESS_TOKEN_URL = "https://<hostname>/v1/oauth2/token";

        static void Main(string[] args)
        {
            Console.WriteLine("Refer-a-friend API");

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Store Group Id: ");
            var storeGroupId = Console.ReadLine();

            var token = Task.Run(() => GetTokenAsync()).Result;
            var oauthToken = JsonConvert.DeserializeObject<OAuthToken>(token);

            var accessToken = oauthToken.access_token;

            //Console.WriteLine($"Access Token: {accessToken}");

            var data = Task.Run(() => GetDataAsync(accessToken, email, storeGroupId)).Result;
            var cta = JsonConvert.DeserializeObject<CallToAction>(data);

            Console.WriteLine($"Title: {cta.title}");
            Console.WriteLine($"Subtitle: {cta.subtitle}");
            Console.WriteLine($"Share URL: {cta.share_url}");
            Console.WriteLine($"Image URL: {cta.image_url}");
        }

        static async Task<string> GetTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", CLIENT_ID),
                    new KeyValuePair<string, string>("client_secret", CLIENT_SECRET)
                };

                HttpContent content = new FormUrlEncodedContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var responseResult = client.PostAsync(ACCESS_TOKEN_URL, content).Result;

                return await responseResult.Content.ReadAsStringAsync();
            }
        }

        static void RefreshToken()
        {
            // TODO:
            //using (var client = new HttpClient())
            //{
            //    var postData = new List<KeyValuePair<string, string>>();
            //    postData.Add(new KeyValuePair<string, string>("refresh_token", _refreshToken));
            //    postData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            //    postData.Add(new KeyValuePair<string, string>("client_id", _clientId));
            //    postData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));

            //    HttpContent content = new FormUrlEncodedContent(postData);
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //    var responseResult = client.PostAsync(_tokenUrl, content).Result;

            //    //return responseResult.Content.ReadAsStringAsync().Result;
            //}
        }

        static async Task<string> GetDataAsync(string accessToken, string email, string storeGroupId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("x-tfg-store-group-id", storeGroupId);
                client.DefaultRequestHeaders.Add("x-tfg-email", email);
                client.DefaultRequestHeaders.Add("x-api-key", API_KEY);

                HttpResponseMessage result = client.GetAsync(GET_URL).Result;

                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //TODO:
                    // refresh token
                    // display results

                    // if (result.StatusCode == HttpStatusCode.Unauthorized)
                    // throw error
                }

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
