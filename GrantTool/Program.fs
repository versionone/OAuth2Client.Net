

open OAuth2Client
open System
open System.IO
open System.Threading.Tasks

let askForAuthCode (client:AuthClient) = 
  let url = client.getUrlForGrantRequest()
  System.Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
  printfn "Please visit this url to authorize the permissions:\n\n%s" (url)
  printfn "\nPaste the code here:"
  Console.ReadLine()

[<EntryPoint>]
let main (argv:string[]) =
  try
    let scope, secretsFilename, credsFilename = 
      match argv.Length with
      | 1 -> argv.[0], "client_secrets.json", "stored_credentials.json"
      | 2 -> argv.[0], argv.[1], "stored_credentials.json"
      | 3 -> argv.[0], argv.[1], argv.[2]
      | _ -> failwith "Invalid command line arguments.\n\nUsage:\n\n GrantTool.exe <scope> [<secretsfile>] [<credsfile>]"

    let secrets = Secrets.FromJson(File.ReadAllText(secretsFilename, Text.Encoding.UTF8))

    let client = AuthClient(secrets, scope, null, null)
    let code = askForAuthCode client
    let creds = client.exchangeAuthCode code

    File.WriteAllText(credsFilename, creds.ToJson())

    printfn "Successfully saved credentials to %s" credsFilename
    0

  with :? Exception as ex ->
    printfn "%s" (ex.StackTrace.ToString())
    printfn "Failed.\n%s" ex.Message
    ignore(Console.ReadLine())
    1
  

