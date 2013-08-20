module OAuth2Client.AuthHandler

open System.Net
open System.Net.Http

open Extensions.HttpExtensions



/// Attempt to parse the "x=y, z=q" style parameters from a WWW-Authenticate header
/// In a 401 from an OAuth2 endpoint, WWW-Authenticate might look like this:
///
///    WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
///
/// See http://tools.ietf.org/html/rfc6750#page-7
let parseParams (param:string) = [
    for p in param.Split(',') do
      let spl = p.Trim().Split( [| '=' |] , 2)
      yield spl.[0], spl.[1]
      ]
       
/// Find any "WWW-Authenticate: Bearer" headers in the response.
let collectBearerParams (response:HttpResponseMessage) =
  dict [
    for header in response.Headers.WwwAuthenticate do
      if header.Scheme = "Bearer" then yield! parseParams header.Parameter
    ]
    
/// We should only refresh if we see the OAuth2 WWW-Authenticate in the 401.
let shouldRefresh (response:HttpResponseMessage) = 
  let parameters = collectBearerParams response
  response.StatusCode = HttpStatusCode.Unauthorized //&& parameters.ContainsKey("error") && parameters.["error"] = "invalid_token"


/// This Handler can be supplied to System.Net.HttpClient to pemit it to work against an OAuth2-protected host.
/// It will add the appropriate Authorization header to requests, and will refresh the OAuth2 access token if
/// the request fails due to a 401.
///
/// The storage will be used to gather the secrets and credentials, and will be used to store the credentials
/// if they end up being refreshed.
type OAuth2BearerHandler(innerHandler:HttpMessageHandler, storage:IStorageAsync, scope:string) =
  inherit DelegatingHandler(innerHandler)

  member this.MySendAsync(req, token) = base.SendAsync(req, token) |> Async.AwaitTask

  override this.SendAsync(req, token) = Async.StartAsTask <| async {
      let! creds = storage.GetCredentialsAsync() |> Async.AwaitTask
      req.AddBearer(creds)
      let! response = this.MySendAsync(req, token)
      if shouldRefresh response then
        let! secrets = storage.GetSecretsAsync() |> Async.AwaitTask
        let authclient = AuthClient(secrets, scope)
        let! newcreds = authclient.refreshAuthCodeAsync(creds) |> Async.AwaitTask
        let! storedcreds = storage.StoreCredentialsAsync(newcreds)  |> Async.AwaitTask
        req.AddBearer(storedcreds)
        return! this.MySendAsync(req, token)
      else
        return response
    }
    