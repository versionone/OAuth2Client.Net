module OAuth2Client.Listener

open System.Text
open System.Net

type HttpListenerContext with

  member this.Respond(status:int, ?body, (?headers:(string*string) list)) = async {
    let body = defaultArg body ""
    let headers = defaultArg headers []
    for name, value in headers do
      this.Response.Headers.Add(name, value)
    if body <> "" then
      let bytes = Encoding.UTF8.GetBytes(body)
      this.Response.ContentLength64 <- bytes.LongLength
      let! successflag = this.Response.OutputStream.WriteAsync(bytes,0,bytes.Length) |> Async.AwaitIAsyncResult
      ignore successflag
    }

  member this.RequestBodyAsync() = async {
    use reader = new System.IO.StreamReader(this.Request.InputStream, Encoding.UTF8)
    return! reader.ReadToEndAsync() |> Async.AwaitTask
    }

  member this.ReadAuthCode() = async {
    let! formdata = async {
      if this.Request.HttpMethod = "POST" then
        return! this.RequestBodyAsync()
      else
        return this.Request.Url.Query
      }
    let oauth2params =  System.Web.HttpUtility.ParseQueryString(formdata)    
    return oauth2params.["code"], oauth2params.["state"]
    }



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

