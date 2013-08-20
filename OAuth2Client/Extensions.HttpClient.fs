module OAuth2Client.Extensions.HttpClientExtensions

type System.Net.Http.HttpClient with
  /// Create a new HttpClient with the Oauth2 auth handler already plugged in.
  /// The storage will be used to gather the existing secrets and credentials,
  /// and to store any newly refreshed credentials from the OAuth2 host.
  /// If no inner handler is specified, a new System.Net.Http.HttpClientHandler
  /// will be created to handle requests.
  static member WithOAuth2(scope:string, ?storage:OAuth2Client.IStorageAsync, ?innerHandler) = async {
    let storage = defaultArg storage (upcast OAuth2Client.Storage.JsonFileStorage.Default)
    let innerHandler = defaultArg innerHandler (new System.Net.Http.HttpClientHandler())
    let oauth2handler = new OAuth2Client.AuthHandler.OAuth2BearerHandler(innerHandler, storage, scope)
    return new System.Net.Http.HttpClient(oauth2handler)
    }