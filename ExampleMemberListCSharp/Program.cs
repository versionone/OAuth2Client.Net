using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Mono.Options;
using OAuth2Client;

namespace ExampleMemberListCSharp
{
	public class Args
	{
		public string Scope = "apiv1";
		public string ServerUrl = "http://localhost/VersionOne.Web";
		public string ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json";

		public bool Parse(IList<string> args)
		{
			var help = false;

			var opts = new OptionSet {
                {
                    "url|u=", "Server URL to connect to. Default: " + ServerUrl,
                    o => ServerUrl = o
                },
                {
                    "scope|s=", "Scope name your client needs. Default: " + Scope,
                    o => Scope = o
                },
                {
                    "query|q=", "API query URI to issue, Default: " + ApiQuery,
                    o => ApiQuery = o
                },
                {
                    "h|help", "show this message and exit", o => help = o != null
                },
            };

			var exename = Process.GetCurrentProcess().ProcessName;

			try
			{
				opts.Parse(args);
			}
			catch (OptionException e)
			{
				Console.Write(exename + " usage: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try '" + exename + " --help' for more information.");
			}
			if (help)
			{
				Console.WriteLine("Usage: " + exename + " [OPTIONS]");
				Console.WriteLine("Executes a query against VersionOne using OAuth2.");
				Console.WriteLine();
				Console.WriteLine("Options:");
				opts.WriteOptionDescriptions(Console.Out);
				return false;
			}

			return true;
		}
	}

	internal static class WebClientExtensions
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
					var authclient = new AuthClient(secrets, scope, null, null);
					var newcreds = authclient.refreshAuthCode(creds);
					var storedcreds = storage.StoreCredentials(newcreds);
					client.AddBearer(storedcreds);
					return client.DownloadString(path);
				}
				throw;
			}
		}
	}


	internal class AsyncProgram
	{
		private readonly Args _arguments;

		public AsyncProgram(Args arguments)
		{
			_arguments = arguments;
		}

		private async Task<string> DoRequestAsync(string path)
		{
			var httpclient = HttpClientFactory.WithOAuth2(_arguments.Scope);
			var response = await httpclient.GetAsync(_arguments.ServerUrl + _arguments.ApiQuery);
			var body = await response.Content.ReadAsStringAsync();
			return body;
		}

		public int MainAsync()
		{
			var t = DoRequestAsync(_arguments.ServerUrl + _arguments.ApiQuery);
			Task.WaitAll(t);
			Console.WriteLine(t.Result);
			return 0;
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var arguments = new Args();

			if (arguments.Parse(args))
			{
				try
				{
					IStorage storage = Storage.JsonFileStorage.Default;
					using (var webclient = new WebClient())
					{
						Console.WriteLine("Issuing query with this configuration: ");
						Console.WriteLine("Server: " + arguments.ServerUrl);
						Console.WriteLine("OAuth2 scope: " + arguments.Scope);
						Console.WriteLine("API Query: " + arguments.ApiQuery);
						Console.WriteLine();

						var body = webclient.DownloadStringOAuth2(storage, arguments.Scope,
																  arguments.ServerUrl + arguments.ApiQuery);
						Console.WriteLine(body);
					}
					Console.WriteLine();
					Console.WriteLine("Press any key to execute the query again using the Async version...");
					Console.ReadLine();
					new AsyncProgram(arguments).MainAsync();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("Encountered an exception when trying to run the program:");
					Console.Error.WriteLine("Exception type:" + ex.GetType().Name);
					Console.Error.WriteLine("\nException message:\n\n" + ex.Message);
					Console.Error.WriteLine("\nException stack trace:\n\n" + ex.StackTrace);
				}
			}
		}
	}
}

