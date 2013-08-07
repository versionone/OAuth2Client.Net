// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.



open OAuth2Client

open Nito.AsyncEx.Synchronous

module Defaults =
  let Scope = "apiv1"
  //let EndpointUrl = "https://www7.v1host.com/V1Production"
  let EndpointUrl = "http://localhost/VersionOne.Web"
  let ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json;format=simple"


let mainAsync () = async {
  let storage : IStorageAsync = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let! secrets = Async.AwaitTask <| storage.GetSecretsAsync()
  let! creds = Async.AwaitTask <| storage.GetCredentialsAsync()
  let client = OAuth2Client.AuthClient(secrets, Defaults.Scope)
  use clientHandler = new System.Net.Http.HttpClientHandler()
  use oauth2handler = new  OAuth2Client.AuthHandler.OAuth2BearerHandler(clientHandler, storage, creds, client)
  let httpclient = new System.Net.Http.HttpClient(oauth2handler)
  let! response = Async.AwaitTask <| httpclient.GetAsync(Defaults.EndpointUrl + Defaults.ApiQuery)
  let! body = Async.AwaitTask <| response.Content.ReadAsStringAsync()
  let results = body
  printfn "%s" results
  return 0
  }

open Extensions.Http

let main () =
  let storage : IStorage = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let secrets = storage.GetSecrets()
  let creds = storage.GetCredentials()
  let client = OAuth2Client.AuthClient(secrets, Defaults.Scope)
  use webclient = new System.Net.WebClient()
  webclient.AddBearer(creds)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery
  let response =
    try
      webclient.DownloadString(url)
    with
      :? System.Net.WebException as ex ->
        let creds = client.refreshAuthCode(creds)
        webclient.AddBearer(creds)
        webclient.DownloadString(url)
  printfn "%s" response
  0
  



[<EntryPoint>]
//let syncMain argv = Async.StartAsTask(main()).WaitAndUnwrapException()
let syncMain argv = main()

  