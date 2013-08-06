module OAuth2Client.AuthHandler

open System.Net
open System.Net.Http

open Extensions.Http


let parseParams (param:string) = [
    for p in param.Split(',') do
      let spl = p.Split( [| '=' |] , 2)
      yield spl.[0], spl.[1]
      ]
       
let collectBearerParams (response:HttpResponseMessage) =
  dict [
    for header in response.Headers.WwwAuthenticate do
      if header.Scheme = "Bearer" then yield! parseParams header.Parameter
    ]
    
let shouldRefresh (response:HttpResponseMessage) = 
  let parameters = collectBearerParams response
  response.StatusCode = HttpStatusCode.Unauthorized //&& parameters.ContainsKey("error") && parameters.["error"] = "invalid_token"




type OAuth2BearerHandler(innerHandler:HttpMessageHandler, storage:IStorage, creds:Credentials, authclient:AuthClient) =
  inherit DelegatingHandler(innerHandler)

  member this.MySendAsync(req, token) = base.SendAsync(req, token) |> Async.AwaitTask

  override this.SendAsync(req, token) = Async.StartAsTask <| async {
      req.AddBearer(creds)
      let! response = this.MySendAsync(req, token)
      if shouldRefresh response then
        let! newcreds = authclient.refreshAuthCode(creds) |> Async.AwaitTask
        let! storedcreds = storage.StoreCredentials(newcreds)  |> Async.AwaitTask
        req.AddBearer(storedcreds)
        return! this.MySendAsync(req, token)
      else
        return response
    }
    