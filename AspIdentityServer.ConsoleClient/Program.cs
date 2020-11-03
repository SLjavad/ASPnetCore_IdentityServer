using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspIdentityServer.ConsoleClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            RunnerAsync().GetAwaiter().GetResult();
        }

        private static async Task RunnerAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var discovery = await httpClient.GetDiscoveryDocumentAsync("http://localhost:5000");
                if (discovery.IsError)
                {
                    Console.WriteLine(discovery.Error);
                    return;
                }
                var tokenEndpoint = discovery.TokenEndpoint;
                //var tokenRequest = new ClientCredentialsTokenRequest
                //{
                //    Address = tokenEndpoint,
                //    ClientId = "client",
                //    ClientSecret = "secret",
                //    Scope = "AspIdentityServer"
                //};

                //var tokenResp = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);
                //Console.WriteLine(tokenResp.Json);
                //Console.WriteLine();

                var tokenRequest = new PasswordTokenRequest 
                {
                    Address = tokenEndpoint,
                    ClientId = "ro.client",
                    ClientSecret = "secret",
                    Scope = "AspIdentityServer",
                    UserName = "javad",
                    Password = "123"
                };
                var tokenResp = await httpClient.RequestPasswordTokenAsync(tokenRequest);
                Console.WriteLine(tokenResp.Json);
                Console.WriteLine();

                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(tokenResp.AccessToken);

                    string inp;
                    while ((inp = Console.ReadLine()) != "stop")
                    {
                        switch (inp)
                        {
                            case "post":
                                var custInfo = new StringContent(JsonConvert.SerializeObject(
                                    new { FirstName = "ali", LastName = "alavi" }
                                    ),Encoding.UTF8 , "application/json");

                                var resp = await client.PostAsync("http://localhost:19755/api/customers", custInfo);
                                if (resp.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("User Created");
                                }
                                else
                                {
                                    Console.WriteLine("Error in Creating User");
                                    Console.WriteLine($"status code is {resp.StatusCode}");
                                }
                                break;
                            case "getAll":
                                var resp2 = await client.GetAsync("http://localhost:19755/api/customers");
                                if (resp2.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"{JArray.Parse(resp2.Content.ReadAsStringAsync().Result)}");
                                }
                                else
                                {
                                    Console.WriteLine("Error in Creating User");
                                    Console.WriteLine($"status code is {resp2.StatusCode}");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
