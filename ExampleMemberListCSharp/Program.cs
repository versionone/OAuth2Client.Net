using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;	
using System.Text;
using System.Threading.Tasks;
using OAuth2Client;
using OAuth2Client.Extensions;

namespace ExampleMemberListCSharp
{
	class Defaults
	{
		public static string Scope = "apiv1";
		public static string EndpointUrl = "http://localhost/VersionOne.Web";
		public static string ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json;format=simple";
	}

	class AsyncProgram
	{
		private static async Task<string> DoRequestAsync(string path)
		{
			var httpclient = NewHttpClient(Storage.JsonFileStorage.Default, Defaults.Scope);
			var response = await httpclient.GetAsync(Defaults.EndpointUrl + Defaults.ApiQuery);
			var body = await response.Content.ReadAsStringAsync();
			return body;
		}

		private static System.Net.Http.HttpClient NewHttpClient(IStorageAsync storage = null, string scope = null,
		                                                        HttpMessageHandler inner = null)
		{
			var myStorage = storage ?? Storage.JsonFileStorage.Default;
			var myScope = scope ?? Defaults.Scope;
			var myHandler = inner ?? new HttpClientHandler();
			var oauth2Handler = new AuthHandler.OAuth2BearerHandler(myHandler, myStorage, myScope);
			return new System.Net.Http.HttpClient(oauth2Handler);
		}

		public static int MainAsync(string[] args)
		{
			var t = DoRequestAsync(Defaults.EndpointUrl + Defaults.ApiQuery);
			Task.WaitAll(t);
			Console.WriteLine(t.Result);
			return 0;
		}
	}


	class Program
	{		
		static string DoRequest(string path)
		{
			IStorage storage = Storage.JsonFileStorage.Default;
			var creds = storage.GetCredentials();
			using (var webclient = new WebClient())
			{
				webclient.Headers["Authorization"] =
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken).ToString();
				try
				{
					return webclient.DownloadString(path);
				}
				catch(WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						var response = (HttpWebResponse)ex.Response;
						if (response.StatusCode == HttpStatusCode.Unauthorized)
						{
							var secrets = storage.GetSecrets();
							var authclient = new AuthClient(secrets, "apiv1");
							var newcreds = authclient.refreshAuthCode(creds);
							var storedcreds = storage.StoreCredentials(newcreds);
							webclient.Headers["Authorization"] =
								new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", storedcreds.AccessToken).ToString();
							return webclient.DownloadString(path);
						}
					}
					throw;
				}
			}
		}

		static void Main(string[] args)
		{
			var body = DoRequest(Defaults.EndpointUrl + Defaults.ApiQuery);

			Console.WriteLine(body);

			Console.ReadLine();
			AsyncProgram.MainAsync(args);
		}
	}
}
  
