module AuthModuleTests

open System.Net
open NUnit.Framework
open OAuth2Client



let TESTURL = "http://localhost/VersionOne.Web/rest-1.v1/Data/Member"

/// This test depends on suitable client_secrets.json and stored_credentials.json
/// files being present and suitable for an instance currently running on localhost/VersionOne.Web

let [<Test>] ``The authorization module works with a System.Net.WebRequest`` () =
  AuthenticationManager.Unregister("basic")
  AuthenticationManager.Register(OAuth2BearerModule())
  let creds = OAuth2Credentials("apiv1", Storage.JsonFileStorage.Default, null)

  let req = WebRequest.CreateHttp(TESTURL)
  req.Credentials <- creds
  req.PreAuthenticate <- true
  req.AllowAutoRedirect <- true
  let resp = req.GetResponse()
  use stream = new System.IO.StreamReader(resp.GetResponseStream())
  let body = stream.ReadToEnd()
  
  // with the System.Net client, the first request will NOT pre-authenticate
  // because it wants to first see an Authorization from a successful Authenticate()
  // to do so. So we run the request twice just to verify the second time does
  // properly pre-authenticate and doesn't refresh.

  // Uh, i guess we dont actually check that here but this test will drive
  // traffic that can be checked with a packet capture
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
