

open OAuth2Client
open System
open System.IO


let askForAuthCode (client:AuthClient) = 
  let url = client.getUrlForGrantRequest()
  System.Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
  printfn "Please visit this url to authorize the permissions:\n\n%s" (url)
  printfn "\nPaste the code here:"
  Console.ReadLine()


let asyncMain (argv:string[]) = async {

  let scope, secretsFilename, credsFilename = 
    match argv.Length with
    | 2 -> argv.[1], "client_secrets.json", "stored_credentials.json"
    | 3 -> argv.[1], argv.[2], "stored_credentials.json"
    | 4 -> argv.[1], argv.[2], argv.[3]
    | _ -> failwith "Usage: %s <scope> [<secretsfile>] [<credsfile>]" argv.[0]

  let secrets = Secrets.FromJson(File.ReadAllText(secretsFilename, Text.Encoding.UTF8))

  let client = AuthClient(secrets, scope)
  let code = askForAuthCode client
  let! creds = Async.AwaitTask <| client.exchangeAuthCode code

  File.WriteAllText(credsFilename, creds.ToJson())

  printfn "Successfully saved credentials to %s" credsFilename
  return 0
  }


[<EntryPoint>]
let main =  asyncMain >> Async.RunSynchronously

