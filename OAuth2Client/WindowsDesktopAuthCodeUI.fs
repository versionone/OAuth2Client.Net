namespace OAuth2Client

open System.Net
open System.Text


open Extensions.HttpListener

type DesktopAuthRedirectUI =

  member this.ListenForOauth2Code(urls:string list) : Async<HttpListenerContext> = async {
    use listener = new HttpListener()
    for url in urls do
      if not(url.StartsWith("urn")) then
        listener.Prefixes.Add(url)
    listener.Start()
    return! listener.GetContextAsync() |> Async.AwaitTask
    }


  member this.GetAuthCode(granturl:string, redirectUrls) = async {
    let browserprocess = System.Diagnostics.Process.Start(granturl)
    let! httprequest =  this.ListenForOauth2Code(redirectUrls)
    let! code, state = httprequest.ReadAuthCode()
    return code
    }

