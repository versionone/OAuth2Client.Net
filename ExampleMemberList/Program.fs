
open System.Net
open Nito.AsyncEx.Synchronous

open OAuth2Client
open OAuth2Client.Extensions.Http

module Defaults =
  let Scope = "apiv1"
  //let EndpointUrl = "https://www7.v1host.com/V1Production"
  let EndpointUrl = "http://localhost/VersionOne.Web"
  let ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json;format=simple"




let asyncMain () = async {
  let storage : IStorageAsync = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let! secrets = Async.AwaitTask <| storage.GetSecretsAsync()
  let! creds = Async.AwaitTask <| storage.GetCredentialsAsync()
  let client = OAuth2Client.AuthClient(secrets, Defaults.Scope)
  use clientHandler = new Http.HttpClientHandler()
  use oauth2handler = new  OAuth2Client.AuthHandler.OAuth2BearerHandler(clientHandler, storage, creds, client)
  let httpclient = new Http.HttpClient(oauth2handler)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery
  let! response = Async.AwaitTask <| httpclient.GetAsync(url)
  let! body = Async.AwaitTask <| response.Content.ReadAsStringAsync()
  let results = body
  printfn "%s" results
  return 0
  }


let main () =
  let storage : IStorage = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let secrets = storage.GetSecrets()
  let creds = storage.GetCredentials()
  let client = OAuth2Client.AuthClient(secrets, Defaults.Scope)
  use webclient = new WebClient()
  webclient.AddBearer(creds)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery
  // we can't plug into WebClient to handle Unauthorized exceptions so we have to do it ourselves.
  let response =
    try
      webclient.DownloadString(url)
    with
      :? WebException as ex when ex.Status = WebExceptionStatus.ProtocolError ->
        let creds = client.refreshAuthCode(creds)
        webclient.AddBearer(creds)
        webclient.DownloadString(url)
  printfn "%s" response
  0
  


[<EntryPoint>]
let syncMain argv =
  ignore(main()) // run it once
  Async.StartAsTask(asyncMain()).WaitAndUnwrapException() // run it again as async

  