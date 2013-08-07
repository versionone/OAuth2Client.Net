﻿module OAuth2Client.Storage

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
    override this.GetSecrets() = 
      Secrets.FromJson <| File.ReadAllText(secretsFileName)

    override this.GetCredentials() = 
      Credentials.FromJson <| File.ReadAllText(credFileName)

    override this.StoreCredentials(creds:Credentials) = 
      File.WriteAllText(credFileName,  creds.ToJson())
      creds

  interface IStorageAsync with
    override this.GetSecretsAsync() = Async.StartAsTask <| async {
      let! text = File.ReadTextAsync(secretsFileName)
      return Secrets.FromJson(text)
      }

    override this.GetCredentialsAsync() = Async.StartAsTask <| async {
      let! text = File.ReadTextAsync(credFileName)
      return Credentials.FromJson(text)
      }
      
    override this.StoreCredentialsAsync(creds:Credentials) = Async.StartAsTask <| async {
      let! result = File.WriteTextAsync(credFileName, creds.ToJson())
      return creds
      }

  static member Default = JsonFileStorage(Options.FromEnvironment.secretsFile, Options.FromEnvironment.credsFile)

