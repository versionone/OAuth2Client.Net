
#r "bin/Debug/OAuth2Client.dll";;


let storage = OAuth2Client.Storage.JsonFiles()
let flow = OAuth2Client.Flow(storage)
let secrets, credentials = Async.RunSynchronously <| flow.Start("query-api-1.0")
printfn "%A\n%A" secrets credentials