namespace OAuth2Client


open System


type Flow(storage:Storage.JsonFiles) = 

  member this.GetOutOfBand(secrets:Secrets, scope) =
    Console.SetIn(new IO.StreamReader(Console.OpenStandardInput(8192)));
    let client = AuthClient(secrets, scope)
    printfn "Please visit this url to authorize the permissions:\n\n%s" (client.getUrlForGrantRequest())
    printfn "\nPaste the code here:"
    let code = Console.ReadLine()
    client.exchangeAuthCode(code)

  member this.GetWeb(secrets:Secrets, scope) = 
    failwith "NotImplemented"

  member this.Start(scope) = async {

    let! secrets = async {
      try
        return! storage.GetSecrets()
      with
      | :? IO.IOException as ex ->
        return! failwith (sprintf "Exception %A reading secrets file %s. Please download or create secrets file and try again" ex storage.SecretsFilename)
      }

    let! credentials = async {
      try
        return! storage.GetCredentials()
      with
      | :? System.IO.IOException as ex ->
        let! creds = 
          match secrets.client_type with
          | Installed -> this.GetOutOfBand(secrets, scope)
          | Web -> this.GetWeb(secrets, scope)
        return! storage.StoreCredentials(creds)
        }

    return secrets, credentials
    }
