module OAuth2Client.Storage

open FSharp.Data.Json
open FSharp.Data.Json.Extensions

open System.IO
open System.Text
open System

open Extensions.File

type Options = { secretsFile: string; credsFile: string } with

  static member Default =
    { secretsFile = "client_secrets.json"
      credsFile = "stored_credentials.json" }

  static member FromEnvironment =
    let envSecrets = Environment.GetEnvironmentVariable("V1SDK_SECRETS")
    let envCreds = Environment.GetEnvironmentVariable("V1SDK_CREDS")
    { secretsFile = if envSecrets = null then Options.Default.secretsFile else envSecrets
      credsFile = if envCreds = null then Options.Default.credsFile else envCreds
      }
     

type JsonFileStorage(secretsFileName:string, credFileName:string) =
  interface IStorage with

    member this.GetSecrets() = Async.StartAsTask <| async {
      let! text = File.ReadTextAsync(secretsFileName)
      return Secrets.FromJson(text)
      }

    member this.GetCredentials() = Async.StartAsTask <| async {
      let! text = File.ReadTextAsync(credFileName)
      return Credentials.FromJson(text)
      }
      
    member this.StoreCredentials(creds:Credentials) = Async.StartAsTask <| async {
      let! result = File.WriteTextAsync(credFileName, creds.ToJson())
      return creds
      }

  static member Default = JsonFileStorage(Options.FromEnvironment.secretsFile, Options.FromEnvironment.credsFile)

