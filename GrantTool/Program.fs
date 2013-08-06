

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


let asyncMain (argv:string[]) = async {
  try

    let scope, secretsFilename, credsFilename = 
      match argv.Length with
      | 1 -> argv.[0], "client_secrets.json", "stored_credentials.json"
      | 2 -> argv.[0], argv.[1], "stored_credentials.json"
      | 3 -> argv.[0], argv.[1], argv.[2]
      | _ -> failwith "Usage: GrantTool.exe <scope> [<secretsfile>] [<credsfile>]"

    let secrets = Secrets.FromJson(File.ReadAllText(secretsFilename, Text.Encoding.UTF8))

    let client = AuthClient(secrets, scope)
    let code = askForAuthCode client
    let! creds = Async.AwaitTask <| client.exchangeAuthCode code

    File.WriteAllText(credsFilename, creds.ToJson())

    printfn "Successfully saved credentials to %s" credsFilename
    return 0

  with :? Exception as ex ->
    printfn "%s" (ex.StackTrace.ToString())
    printfn "Failed.\n%s" ex.Message
    ignore(Console.ReadLine())
    return 1
  }

open Nito.AsyncEx.Synchronous

[<EntryPoint>]
let main argv = 
  let t =  asyncMain argv |> Async.StartAsTask
  t.WaitAndUnwrapException()

