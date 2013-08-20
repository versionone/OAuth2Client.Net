module OAuth2Client.Extensions.FileExtensions

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