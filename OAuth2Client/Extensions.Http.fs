module OAuth2Client.Extensions.Http

open System
open System.Net
open System.Net.Http
open System.Text


type WebClient with
  member this.AddBearer(creds:OAuth2Client.Credentials) =
    this.Headers.["Authorization"] <- Http.Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken).ToString()


type HttpWebRequest with
  member this.AddBearer(creds:OAuth2Client.Credentials) = 
    this.Headers.["Authorization"] <- Http.Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken).ToString()



type HttpRequestMessage with
  member this.AddBearer(creds:OAuth2Client.Credentials) =
    this.Headers.Authorization <- Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken)



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

  member this.RequestBodyAsync() : Async<string> = async {
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



