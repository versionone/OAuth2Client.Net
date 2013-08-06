// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.



open OAuth2Client

open Nito.AsyncEx.Synchronous

module Defaults =
  let Scope = "apiv1"
  //let EndpointUrl = "https://www7.v1host.com/V1Production"
  let EndpointUrl = "http://localhost/VersionOne.Web"
  let ApiQuery = "/rest-1.v1/Data/Member?Accept=text/json;format=simple"


let main () = async {
  let storage : IStorage = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let! secrets = Async.AwaitTask <| storage.GetSecrets()
  let! creds = Async.AwaitTask <| storage.GetCredentials()
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


[<EntryPoint>]
let syncMain argv = (main() |> Async.StartAsTask).WaitAndUnwrapException()

  