namespace OAuth2Client

open System
open System.Collections.Specialized
open System.Web

open FSharp.Data.Json
open FSharp.Data.Json.Extensions

type AuthClient(secrets:Secrets, scope:string) = 
  let encodeQueryString (nvs:(string * string) seq) =
    String.Join("&", [for n,v in nvs -> (sprintf "%s=%s" n (HttpUtility.UrlEncode v))])

  let toNameValueCollection l =
    let nvc = new NameValueCollection()
    for n, v in l do
      nvc.Add(n, v)
    nvc
      
  member this.getUrlForGrantRequest() =
    let u = new UriBuilder(secrets.auth_uri)
    u.Query <- encodeQueryString [
                "response_type", "code"
                "client_id", secrets.client_id
                "redirect_uri", secrets.redirect_uris.Head.ToString()
                "scope", scope
                "state", ""
                ]
    u.ToString()

  member this.doAuthRequest(parameters) = async {
    let webClient = new System.Net.WebClient()
    let postBody = toNameValueCollection parameters
    printfn "Posting to %s" (secrets.auth_uri.ToString())
    let! resultbytes =  Async.AwaitTask <| webClient.UploadValuesTaskAsync(secrets.token_uri, "POST", postBody)
    let result = System.Text.Encoding.UTF8.GetString(resultbytes)
    printfn "%s" result
    return Credentials.FromJson(result)
    }

  member this.exchangeAuthCode(code:string) =
    this.doAuthRequest(
      [ "code", code
        "client_id", secrets.client_id
        "client_secret", secrets.client_secret
        "redirect_uri", secrets.redirect_uris.Head.ToString()
        "scope", scope
        "grant_type", "authorization_code"
        ] 
      )

  member this.refreshAuthCode(creds:Credentials) = 
    this.doAuthRequest(
      [ "refresh_token", creds.RefreshToken
        "client_id", secrets.client_id
        "client_secret", secrets.client_secret
        "grant_type", "refresh_token"
        ]
      )
    