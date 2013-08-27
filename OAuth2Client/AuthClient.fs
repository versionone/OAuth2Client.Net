namespace OAuth2Client

open System
open System.Collections.Specialized
open System.Collections.Generic
open System.Web
open System.Net

open System.Runtime.InteropServices

type AuthClient(secrets:Secrets, scope:string, [<Optional;DefaultParameterValue(null)>]?proxy, [<Optional;DefaultParameterValue(null)>]?handler) = 
  let await = Async.AwaitTask
  let start = Async.StartAsTask

  let encodeQueryString (nvs:(string * string) seq) =
    String.Join("&", [for n,v in nvs -> (sprintf "%s=%s" n (HttpUtility.UrlEncode v))])

  let toNameValueCollection l =
    let nvc = new NameValueCollection()
    for n, v in l do
      nvc.Add(n, v)
    nvc

  member this.refreshToken(creds) = [
    "refresh_token", creds.RefreshToken
    "client_id", secrets.client_id
    "client_secret", secrets.client_secret
    "scope", scope
    "grant_type", "refresh_token"
    ]

  member this.authToken(code) = [
    "code", code
    "client_id", secrets.client_id
    "client_secret", secrets.client_secret
    "redirect_uri", secrets.redirect_uris.Head.ToString()
    "scope", scope
    "grant_type", "authorization_code"  ] 
      
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
    
  member this.doAuthRequestAsync(parameters) = start <| async {
    let handler = 
      if handler.IsSome then handler.Value else
      let h = new System.Net.Http.HttpClientHandler()
      if proxy.IsSome then
        h.Proxy <- proxy.Value
        h.UseProxy <- true
      h
    use httpclient = new System.Net.Http.HttpClient(handler)
    let postBody = toNameValueCollection parameters
    let postBody = new Http.FormUrlEncodedContent([for name,value in parameters -> new KeyValuePair<_,_>(name,value)])
    let! response = await <| httpclient.PostAsync(secrets.token_uri, postBody)
    let! responseBody =  await <| response.Content.ReadAsStringAsync()
    return Credentials.FromJson(responseBody)
    }

  member this.doAuthRequest(parameters) =
    use webclient = new System.Net.WebClient()
    match proxy with
    | Some p -> webclient.Proxy <- p
    |_ -> ()
    let postBody = toNameValueCollection parameters
    let response = webclient.UploadValues(secrets.token_uri, postBody)
    let responseBody =  System.Text.Encoding.UTF8.GetString(response)
    Credentials.FromJson(responseBody)
    
  member this.exchangeAuthCode(code) = code |> this.authToken |> this.doAuthRequest
  member this.exchangeAuthCodeAsync(code) = code |> this.authToken |> this.doAuthRequestAsync
  member this.refreshAuthCode(creds) = creds |> this.refreshToken |> this.doAuthRequest
  member this.refreshAuthCodeAsync(creds)= creds |> this.refreshToken |> this.doAuthRequestAsync
      