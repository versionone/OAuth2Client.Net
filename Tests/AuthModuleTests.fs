﻿module AuthModuleTests

open NUnit.Framework
open OAuth2Client

open System.Net

let SECRETS = @"C:\Users\JKoberg\support\ssotest\localhost\client_secrets.json"
let CREDS = @"C:\Users\JKoberg\support\ssotest\localhost\stored_credentials.json"

let [<Test>] ``The authorization module is usable`` () = 
  let storage = Storage.JsonFileStorage(SECRETS, CREDS)
  let m = AuthModule() // default storage will look in PWD or file supplied by envar
  AuthenticationManager.Unregister("basic")
  AuthenticationManager.Register(m)
  let req = WebRequest.CreateHttp("http://jkoberg1/VersionOne.Web/rest-1.v1/Data/Member")
  req.PreAuthenticate <- true
  req.Credentials <- OAuth2Credentials("apiv1", storage)
  req.AllowAutoRedirect <- true
  let resp = req.GetResponse()
  let stream = new System.IO.StreamReader(resp.GetResponseStream())
  let body = stream.ReadToEnd()
  printfn "%A" resp
  printfn "%s" body
  ()
