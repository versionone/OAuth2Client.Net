using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;	
using System.Text;
using System.Threading.Tasks;
using OAuth2Client;

namespace ExampleMemberListCSharp
{
	class Defaults
	{
		public static string Scope = "apiv1";
		public static string EndpointUrl = "http://localhost/VersionOne.Web";
		public static string ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json";
	}


	static class WebClientExtensions
	{
		public static string DownloadStringOAuth2(this WebClient client, IStorage storage, string scope, string path)
		{
			var creds = storage.GetCredentials();
			client.AddBearer(creds);
			try
			{
				return client.DownloadString(path);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					if (((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.Unauthorized)
						throw;
					var secrets = storage.GetSecrets();
					var authclient = new AuthClient(secrets, scope);
					var newcreds = authclient.refreshAuthCode(creds);
					var storedcreds = storage.StoreCredentials(newcreds);
					client.AddBearer(storedcreds);
					return client.DownloadString(path);
				}
				throw;
			}
		}
	}


	class AsyncProgram
	{
		private static async Task<string> DoRequestAsync(string path)
		{
			var httpclient = HttpClientFactory.WithOAuth2("apiv1");
			var response = await httpclient.GetAsync(Defaults.EndpointUrl + Defaults.ApiQuery);
			var body = await response.Content.ReadAsStringAsync();
			return body;
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
		static void Main(string[] args)
		{
			IStorage storage = Storage.JsonFileStorage.Default;
			using (var webclient = new WebClient())
			{
				var body = webclient.DownloadStringOAuth2(storage, "apiv1", Defaults.EndpointUrl + Defaults.ApiQuery);
				Console.WriteLine(body);
			}
			Console.ReadLine();
			AsyncProgram.MainAsync(args);
		}
	}
}
  
