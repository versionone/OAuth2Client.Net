namespace OAuth2Client
open System.Runtime.InteropServices

type HttpClientFactory = 
  static member WithOAuth2(scope, [<Optional;DefaultParameterValue(null)>]?storage, [<Optional;DefaultParameterValue(null)>]?handler) =
    let handler = if Option.isNone handler then new System.Net.Http.HttpClientHandler() else handler.Value
    let storage : IStorageAsync = defaultArg storage (upcast OAuth2Client.Storage.JsonFileStorage.Default)
    let oauth2handler = new OAuth2Client.AuthHandler.OAuth2BearerHandler(handler, storage, scope)
    new System.Net.Http.HttpClient(oauth2handler)
    