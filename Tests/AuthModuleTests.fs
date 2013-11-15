module AuthModuleTests

open NUnit.Framework
open OAuth2Client

open System.Net

let SECRETS = @"C:\Users\JKoberg\support\ssotest\localhost\client_secrets.json"
let CREDS = @"C:\Users\JKoberg\support\ssotest\localhost\stored_credentials.json"

let [<Test>] ``The authorization module is usable`` () =
  AuthenticationManager.Unregister("basic")
  AuthenticationManager.Register(OAuth2BearerModule())
  let req = WebRequest.CreateHttp("http://jkoberg1/VersionOne.Web/rest-1.v1/Data/Member")
  req.PreAuthenticate <- true
  req.Credentials <- OAuth2Credentials("apiv1", Storage.JsonFileStorage(SECRETS, CREDS))
  req.AllowAutoRedirect <- true
  let resp = req.GetResponse()
  let stream = new System.IO.StreamReader(resp.GetResponseStream())
  let body = stream.ReadToEnd()
  printfn "%A" resp
  printfn "%s" body
  ()
