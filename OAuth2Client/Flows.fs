namespace OAuth2Client


open System
open System.Net

open System.Text
open System.Threading.Tasks



(*


type Flow(authCodeUI: IAuthCodeRetrievalUI, storage:Storage.JsonFiles, scope:string) = 

  member this.GetOutOfBand(secrets:Secrets) = async {
    let client = AuthClient(secrets, scope)
    let! code = authCodeUI.GetAuthCode(client.getUrlForGrantRequest())
    return! client.exchangeAuthCode(code)
    }


  member this.GetWeb(secrets:Secrets) : Async<string * Async<Credentials>> = async {
    let client = AuthClient(secrets, scope)
    let mypath = secrets.redirect_uris
    let credentials = async {
      let urls = [for url in secrets.redirect_uris -> url.ToString()]
      let! httprequest =  this.ListenForOauth2Code(urls)
      let! code, state = httprequest.ReadAuthCode()
      try
        let! creds = client.exchangeAuthCode(code)
        do! httprequest.Respond(200, "<p>Auth code has been exchanged for a persistent token. You may close this browser window.</p>")
        return creds
      with
        | :? WebException as ex ->
          do! httprequest.Respond(500, "Unable to exchange auth code. " + ex.Message)
          return failwith "Couldn't exchange credentials"
      }
    return client.getUrlForGrantRequest(), credentials
    }


  member this.Start(secretsStorage:ISecretsStorage) = async {
    let! secrets = secretsStorage.GetSecrets() |> Async.AwaitTask
    let! credentials = credentialsStorage.


    let! credentials = async {
      try
        return! storage.GetCredentials()
      with
      | :? System.IO.IOException as ex ->
        let! creds = 
          match secrets.client_type with
          | Installed -> this.GetOutOfBand(secrets)
          | Web -> async {
              let! grantUrl, creds = this.GetWeb(secrets)
              printfn "\nStarted HTTP server to listen for auth code."
              printfn "The granting server must be able to hit this machine for the callback to succeed."
              printfn "\nTo grant permissions, visit URL:\n\n  %s\n\n" grantUrl
              return! creds
              }
        return! storage.StoreCredentials(creds)
        }
    return secrets, credentials
    }


    *)