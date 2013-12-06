
open System.Net
open Nito.AsyncEx.Synchronous

open OAuth2Client

module Defaults =
  let Scope = "apiv1"
  let EndpointUrl = "http://localhost/VersionOne.Web"
  let ApiQuery = "/rest-1.oauth.v1/Data/Member?Accept=text/json"



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


  // First, get the stored credentials.
  let storage : IStorage = upcast OAuth2Client.Storage.JsonFileStorage.Default

  // Use the provided extension method to add the credentials 
  AuthenticationManager.Register(OAuth2BearerModule())
  use webclient = new WebClient()
  webclient.Credentials <- OAuth2Credentials("apiv1", storage, null)
  let url = Defaults.EndpointUrl + Defaults.ApiQuery
  let response = webclient.DownloadString(url)
  // Try it, if it's a 401, refresh and try again.
  printfn "%s" response
  0
  

// This simple runs both examples.
[<EntryPoint>]
let syncMain argv =
  ignore(main()) // run it once
  Async.StartAsTask(asyncMain()).WaitAndUnwrapException() // run it again as async

  