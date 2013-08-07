module OAuth2Client.Extensions.HttpClient


open System.Net.Http
open OAuth2Client

type HttpClient with
  static member WithOAuth2(storage:IStorageAsync, scope:string) = async {
    let! secrets = storage.GetSecretsAsync() |> Async.AwaitTask
    let! creds = storage.GetCredentialsAsync()  |> Async.AwaitTask
    let client = AuthClient(secrets, scope)
    use clientHandler = new System.Net.Http.HttpClientHandler()
    use oauth2handler = new  AuthHandler.OAuth2BearerHandler(clientHandler, storage, creds, client)
    return new System.Net.Http.HttpClient(oauth2handler)
    }