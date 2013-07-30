
#r "bin/Debug/OAuth2Client.dll";;

#r "System.Net.Http";;


open System.Net.Http


let uri = new System.Uri("http://localhost/VersionOne.Web/rest-1.oauth.v1/Data/Member")
let scope = "apiv1"

let work = async {
  let storage = OAuth2Client.Storage.JsonFiles()
  let flow = OAuth2Client.Flow(storage)
  let! secrets, creds = flow.Start(scope)
  let authclient = OAuth2Client.AuthClient(secrets, scope)
  let httpclient = new HttpClient()

  httpclient.DefaultRequestHeaders.Authorization <- Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken)
  let! response = async {
    let! firstTry = httpclient.GetAsync(uri) |> Async.AwaitTask
    if firstTry.StatusCode <> System.Net.HttpStatusCode.Unauthorized then
      return firstTry
    else
      let! newCreds = authclient.refreshAuthCode(creds)
      let! storedCreds = storage.StoreCredentials(newCreds)  
      ignore (httpclient.DefaultRequestHeaders.Remove("Authorization"))
      httpclient.DefaultRequestHeaders.Authorization <- Headers.AuthenticationHeaderValue("Bearer", storedCreds.AccessToken)
      return! httpclient.GetAsync(uri) |> Async.AwaitTask
    }
  let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
  printfn "%s" body
  }

work |> Async.RunSynchronously