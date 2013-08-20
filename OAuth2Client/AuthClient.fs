namespace OAuth2Client

open System
open System.Collections.Specialized
open System.Collections.Generic
open System.Web
open System.Net


open Extensions.HttpExtensions

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
    
  member this.doAuthRequestAsync(parameters) = Async.StartAsTask <| async {
    use httpclient = new System.Net.Http.HttpClient()
    let postBody = toNameValueCollection parameters
    let postBody = new Http.FormUrlEncodedContent([for name,value in parameters -> new KeyValuePair<_,_>(name,value)])
    let! response = Async.AwaitTask <| httpclient.PostAsync(secrets.token_uri, postBody)
    let! responseBody =  Async.AwaitTask <| response.Content.ReadAsStringAsync()
    return Credentials.FromJson(responseBody)
    }

    
  member this.doAuthRequest(parameters) =
    use webclient = new System.Net.WebClient()
    let postBody = toNameValueCollection parameters
    let response = webclient.UploadValues(secrets.token_uri, postBody)
    let responseBody =  System.Text.Encoding.UTF8.GetString(response)
    Credentials.FromJson(responseBody)
    

  member this.exchangeAuthCode(code) =
    this.doAuthRequest(
      [ "code", code
        "client_id", secrets.client_id
        "client_secret", secrets.client_secret
        "redirect_uri", secrets.redirect_uris.Head.ToString()
        "scope", scope
        "grant_type", "authorization_code"
        ] 
      )

  member this.exchangeAuthCodeAsync(code) =
    this.doAuthRequestAsync(
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
        "scope", scope
        "grant_type", "refresh_token"
        ]
      )

  member this.refreshAuthCodeAsync(creds:Credentials) =
    this.doAuthRequestAsync(
      [ "refresh_token", creds.RefreshToken
        "client_id", secrets.client_id
        "client_secret", secrets.client_secret
        "scope", scope
        "grant_type", "refresh_token"
        ]
      )
      