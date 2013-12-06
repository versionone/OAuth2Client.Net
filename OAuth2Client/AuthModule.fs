namespace OAuth2Client

open System.Runtime.InteropServices
open System.Net;

/// ICredentials for the OAuth2 auth module.
/// Each HttpWebRequest can be supplied with these credentials to
/// allow it to do OAuth2 exchanges
type OAuth2Credentials(scope, storage:IStorage, proxy) =
  let secrets = storage.GetSecrets()
  let client = AuthClient(secrets, scope, proxy, null)

  member x.GetOAuth2() =
    storage.GetCredentials()

  member x.RefreshOAuth2([<Optional;DefaultParameterValue(null)>]?oldcred) = 
    let oldcred = defaultArg oldcred (storage.GetCredentials())
    let newcred = client.refreshAuthCode(oldcred)
    storage.StoreCredentials(newcred)

  interface ICredentials with
    member x.GetCredential(uri, authType) = null


/// An OAuth2 authentication module for System.Net, supporting OAuth2 clients.
/// An IStorage must be supplied to fetch OAuth2 Secrets and Credentials and to store refreshed Credentials.
/// The scope is a space-separated list of server-defined scopes.

type OAuth2BearerModule() =
                             
  interface IAuthenticationModule with
    member x.AuthenticationType with get() = "Bearer"
    member x.CanPreAuthenticate with get() = true

    /// Pre-authenticate should apply the Bearer header to all outbound requests
    /// If the request comes back with a 401, hopefully the framework will call
    /// Authenticate to cause a refresh to occur.
    member x.PreAuthenticate(request, systemCreds) =
      match systemCreds with
      | :? OAuth2Credentials as creds ->
        let oauth2creds = creds.GetOAuth2()      
        Authorization("Bearer " + oauth2creds.AccessToken)
      | _ -> null

    /// presumably called in response to a WWW-Authenticate challenge from the
    /// http server. Since the Bearer token we possessed had been applied, this
    /// must mean we need to refresh it.
    member x.Authenticate(challenge, request, systemCreds) =
      if not (challenge.StartsWith("Bearer ")) then null else
      match systemCreds with
      | :? OAuth2Credentials as creds ->
        let oauth2creds = creds.RefreshOAuth2()
        Authorization("Bearer " + oauth2creds.AccessToken)
      | _ -> null
