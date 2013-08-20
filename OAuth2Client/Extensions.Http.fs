module OAuth2Client.Extensions.HttpExtensions

open OAuth2Client

open System
open System.Net
open System.Net.Http
open System.Text


let makeHeader (creds:OAuth2Client.Credentials) = 
  Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken)

type WebClient with
  member this.AddBearer(creds:OAuth2Client.Credentials) =
    this.Headers.["Authorization"] <- makeHeader(creds).ToString()


type HttpWebRequest with
  member this.AddBearer(creds:OAuth2Client.Credentials) = 
    this.Headers.["Authorization"] <- makeHeader(creds).ToString()

type HttpRequestMessage with
  member this.AddBearer(creds:OAuth2Client.Credentials) =
    this.Headers.Authorization <- makeHeader creds

