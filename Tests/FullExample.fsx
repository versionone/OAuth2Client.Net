
#r "bin/Debug/OAuth2Client.dll";;

#r "System.Net.Http";;


open System.Net.Http


let uri = new System.Uri("https://www7.v1host.com/V1Production/rest-1.oauth.v1/Data/Member")
let scope = "query-api-1.0"

Async.RunSynchronously (async {
  let storage = OAuth2Client.Storage.JsonFiles()
  let flow = OAuth2Client.Flow(storage)
  let! secrets, creds = flow.Start(scope)
  let authclient = OAuth2Client.AuthClient(secrets, scope)
  let httpclient = new HttpClient()
  httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer " + creds.AccessToken)
  let! response = async {
    let! firstTry = httpclient.GetAsync(uri) |> Async.AwaitTask
    if firstTry.StatusCode <> System.Net.HttpStatusCode.Unauthorized then
      return firstTry
    else
      let! newCreds = authclient.refreshAuthCode(creds)
      let! storedCreds = storage.StoreCredentials(newCreds)  
      ignore (httpclient.DefaultRequestHeaders.Remove("Authorization"))
      httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer " + creds.AccessToken)
      return! httpclient.GetAsync(uri) |> Async.AwaitTask
    }
  let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
  printfn "%s" body
  })


// startup -------------------------
// (defaults) -> async secrets

// auth grant ----------------------
// (scope, secrets) -> granturl
// (scope, secrets) -> async maybe creds
// (scope, secrets, authcode) -> async creds

// normal usage ---------------------
// (creds, queryurl) -> async either (querydata, Unathorized queryurl creds)
// (secrets, oldcreds) -> async newcreds
// (defaults, creds) -> async creds