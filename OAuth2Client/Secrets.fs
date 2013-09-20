namespace OAuth2Client

open System
open System.Collections.Generic
open FSharp.Data.Json
open FSharp.Data.Json.Extensions




// Secrets holds the client config data from the server including
// our client ID and client secret. It also contains the urls for
// the token and auth endpoints.

type SecretsClientType = Installed | Web

type Secrets =
  { client_type : SecretsClientType
    client_id : string
    client_secret : string
    redirect_uris : System.Uri seq
    auth_uri : System.Uri
    token_uri : System.Uri }

  static member FromJson(txt) =
    let secrets = JsonValue.Parse txt
    let client_type, body =
      if secrets.TryGetProperty("web") = None
        then (Installed, secrets?installed)
        else (Web, secrets?web)
    { client_type = client_type
      client_id = body?client_id.AsString()
      client_secret = body?client_secret.AsString()
      auth_uri = System.Uri(body?auth_uri.AsString())
      token_uri = System.Uri(body?token_uri.AsString())
      redirect_uris = [for u in body?redirect_uris.AsArray() -> System.Uri(u.AsString())]
      }

  member this.ToJson() =
    let t = if this.client_type = Installed then "installed" else "web"
    JsonValue.Object(
        Map [
          t, JsonValue.Object(
            Map [
               "client_id", JsonValue.String this.client_id
               "client_secret", JsonValue.String this.client_secret
               "auth_uri", JsonValue.String(this.auth_uri.ToString())
               "token_uri", JsonValue.String(this.token_uri.ToString())
               "redirect_uris", JsonValue.Array [|for u in this.redirect_uris -> JsonValue.String(u.ToString())|]
        ])]).ToString()
