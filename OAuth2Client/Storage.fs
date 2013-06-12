module OAuth2Client.Storage

open FSharp.Data.Json
open FSharp.Data.Json.Extensions

open System.IO
open System.Text

type File with
  static member WriteTextAsync(path:string, text:string) = async {
    use writer = new StreamWriter(path, append=false, encoding=Encoding.UTF8)
    return! writer.WriteAsync(text) |> Async.AwaitIAsyncResult
    }

  static member ReadTextAsync(path:string) = async {
    use reader = new StreamReader(path, Encoding.UTF8)
    return! reader.ReadToEndAsync() |> Async.AwaitTask
    }


type JsonFiles(?directory, ?secretsFilename, ?credsFilename) =
  let directory = Path.GetFullPath(defaultArg directory ".")
  let secretsFilename = Path.Combine(directory, defaultArg secretsFilename "client_secrets.json")
  let credsFilename = Path.Combine(directory, defaultArg credsFilename "stored_credentials.json")

  member this.CredsFilename = credsFilename

  member this.SecretsFilename = secretsFilename
   
  member this.StoreCredentials(creds:Credentials) = async {
    let! result = File.WriteTextAsync(credsFilename, creds.ToJson())
    return creds
    }

  member this.GetCredentials () = async {
    let! text = File.ReadTextAsync(credsFilename)
    return Credentials.FromJson(text)
    }

  member this.StoreSecrets(secrets:Secrets) = async {
    let! result = File.WriteTextAsync(secretsFilename, secrets.ToJson())
    return secrets
    }

  member this.GetSecrets () = async {
    let! text = File.ReadTextAsync(secretsFilename)
    return Secrets.FromJson(text)
    }

