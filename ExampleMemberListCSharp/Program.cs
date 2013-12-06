using System;
using System.Net;
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
			AuthenticationManager.Register(new OAuth2BearerModule());
			using (var webclient = new WebClient())
			{
				webclient.Credentials = new OAuth2Credentials(Defaults.Scope, storage, null);
				var body = webclient.DownloadString(Defaults.EndpointUrl + Defaults.ApiQuery);
				Console.WriteLine(body);
			}
			Console.ReadLine();
			AsyncProgram.MainAsync(args);
		}
	}
}
  
