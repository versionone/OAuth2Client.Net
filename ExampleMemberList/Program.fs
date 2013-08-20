
open System.Net
open Nito.AsyncEx.Synchronous

open OAuth2Client

module Defaults =
  let Scope = "apiv1"
  let EndpointUrl = "http://localhost/VersionOne.Web"
  let ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json;format=simple"



/// This is an example using the Task<T> interfaces from System.Net.Http.HttpClient
let asyncMain () = async {

  // Create a new HttpClient with the provided extension method. Without the "storage" argument, it uses the default JSON file storage implementation
  let httpclient = System.Net.Http.HttpClient.WithOAuth2(Defaults.Scope)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery

  // Just use the client in a normal fashion.  HTTP Authorization will be added, and tokens refreshed on 401's.
  let! response = Async.AwaitTask <| httpclient.GetAsync(url)
  let! body = Async.AwaitTask <| response.Content.ReadAsStringAsync()
  let results = body
  printfn "%s" results
  return 0
  }



/// This is an example using the normal synchronous methods of WebClient
let main () =

  // Since System.Net.WebClient doesn't allow a handler to deal with HTTP behavior, we have to do it ourselves.

  // First, get the stored credentials.
  let storage : IStorage = upcast OAuth2Client.Storage.JsonFileStorage.Default
  let secrets = storage.GetSecrets()
  let creds = storage.GetCredentials()

  // Create a new AuthClient instance
  let client = OAuth2Client.AuthClient(secrets, Defaults.Scope)

  // Use the provided extension method to add the credentials to the Authorization header.
  use webclient = new WebClient()
  webclient.AddBearer(creds)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery

  // Try it, if it's a 401, refresh and try again.
  let response =
    try
      webclient.DownloadString(url)
    with
      :? WebException as ex when ex.Status = WebExceptionStatus.ProtocolError ->
        let response : HttpWebResponse = downcast ex.Response
        if response.StatusCode <> HttpStatusCode.Unauthorized then reraise() else
        let creds = client.refreshAuthCode(creds)
        webclient.AddBearer(creds)
        webclient.DownloadString(url)
  printfn "%s" response
  0
  


// This simple runs both examples.
[<EntryPoint>]
let syncMain argv =
  ignore(main()) // run it once
  Async.StartAsTask(asyncMain()).WaitAndUnwrapException() // run it again as async

  