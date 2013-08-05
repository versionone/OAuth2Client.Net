namespace OAuth2Client


open FSharp.Data.Json
open FSharp.Data.Json.Extensions

type Credentials =
  {
    AccessToken : string
    RefreshToken : string
    // ExpiresIn : System.TimeSpan
    // ObtainedAt: System.DateTime
    TokenType: string
    Scope : string
    RawJson : string
  }

  static member FromJson(txt, ?obtainedAt) =
    let resultData = JsonValue.Parse(txt)
    {
      AccessToken = (resultData?access_token).AsString()
      RefreshToken = resultData?refresh_token.AsString()
      //ExpiresIn = System.TimeSpan.FromSeconds(resultData?expires_in.AsFloat())
      //ObtainedAt =
      //  if resultData.TryGetProperty("obtained_at") = None
      //    then defaultArg obtainedAt System.DateTime.UtcNow
      //    else System.DateTime.Parse(resultData?obtained_at.ToString())
      TokenType = resultData?token_type.AsString()
      Scope = resultData?scope.AsString()
      RawJson = txt
      }

  member this.ToJson () =
    JsonValue.Object(
        Map [  "access_token", JsonValue.String this.AccessToken
               "refresh_token", JsonValue.String this.RefreshToken
               //"expires_in", JsonValue.Float this.ExpiresIn.TotalSeconds
               //"obtained_at", JsonValue.String (this.ObtainedAt.ToString("o"))
               "token_type", JsonValue.String this.TokenType
               "scope", JsonValue.String this.Scope
               "raw_json", JsonValue.String this.RawJson
        ]).ToString()

    