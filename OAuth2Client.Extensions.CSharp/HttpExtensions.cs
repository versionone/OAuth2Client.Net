using System.Net;
using System.Net.Http;

namespace OAuth2Client.Extensions.CSharp
{
	/// <summary>
	/// Provide convenience extension methods for common OAuth2 operations against .NET framework HTTP clients.
	/// </summary>
    public static class HttpExtensions
    {
		public static System.Net.Http.Headers.AuthenticationHeaderValue MakeHeader(Credentials creds)
		{
			return new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken);
		}

		public static void AddBearer(this WebClient client, Credentials creds)
		{
			client.Headers["Authorization"] = MakeHeader(creds).ToString();
		}

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
					client.AddBearer(creds);
					return client.DownloadString(path);
				}
				throw;
			}
		}

		public static void AddBearer(this HttpWebRequest req, Credentials creds)
		{
			req.Headers["Authorization"] = MakeHeader(creds).ToString();
		}

		public static void AddBearer(this HttpRequestMessage client, Credentials creds)
		{
			client.Headers.Authorization = MakeHeader(creds);
		}

		public static System.Net.Http.HttpClient HttpClientWithOAuth2(string scope, IStorageAsync storage = null, HttpMessageHandler innerHandler = null)
		{
			var myStorage = storage ?? Storage.JsonFileStorage.Default;
			var myInnerHandler = innerHandler ?? new HttpClientHandler();
			var oauth2Handler = new AuthHandler.OAuth2BearerHandler(myInnerHandler, myStorage, scope);
			return new System.Net.Http.HttpClient(oauth2Handler);
		}
    }
}


