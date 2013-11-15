module AuthModuleTests

open System.Net
open NUnit.Framework
open OAuth2Client

let SECRETS = @"C:\Users\JKoberg\support\ssotest\localhost\client_secrets.json"
let CREDS = @"C:\Users\JKoberg\support\ssotest\localhost\stored_credentials.json"
let TESTURL = "http://jkoberg1/VersionOne.Web/rest-1.v1/Data/Member"

let [<Test>] ``The authorization module is usable`` () =
  AuthenticationManager.Unregister("basic")
  AuthenticationManager.Register(OAuth2BearerModule())
  let creds = OAuth2Credentials("apiv1", Storage.JsonFileStorage(SECRETS, CREDS))

  let req = WebRequest.CreateHttp(TESTURL)
  req.Credentials <- creds
  req.PreAuthenticate <- true
  req.AllowAutoRedirect <- true
  let resp = req.GetResponse()
  use stream = new System.IO.StreamReader(resp.GetResponseStream())
  let body = stream.ReadToEnd()
  
  // with the System.Net client, the first request will NOT pre-authenticate
  // because it wants to see an Authorization from a successful Authenticate
  // to do so. So we run the request twice just to verify the second time does
  // properly pre-authenticate and doesn't refresh.
  let req = WebRequest.CreateHttp(TESTURL)
  req.Credentials <- creds
  req.PreAuthenticate <- true
  req.AllowAutoRedirect <- true
  let resp = req.GetResponse()
  use stream = new System.IO.StreamReader(resp.GetResponseStream())
  let body = stream.ReadToEnd()

  printfn "%A" resp
  printfn "%s" body
  ()
