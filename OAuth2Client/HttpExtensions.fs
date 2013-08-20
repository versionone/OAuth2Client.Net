namespace OAuth2Client

[<AutoOpen>]
module HttpExtensions = 
  open OAuth2Client

  open System
  open System.Net
  open System.Net.Http
  open System.Text


  let makeHeader (creds:OAuth2Client.Credentials) = 
    Headers.AuthenticationHeaderValue("Bearer", creds.AccessToken)

  type WebClient with
    member this.AddBearer(creds:OAuth2Client.Credentials) =
      this.Headers.["Authorization"] <- makeHeader(creds).ToString()

  type HttpWebRequest with
    member this.AddBearer(creds:OAuth2Client.Credentials) = 
      this.Headers.["Authorization"] <- makeHeader(creds).ToString()

  type HttpRequestMessage with
    member this.AddBearer(creds:OAuth2Client.Credentials) =
      this.Headers.Authorization <- makeHeader creds

[<AutoOpen>]
[<System.Runtime.CompilerServices.Extension>]
module WebClientExtensions = 
  open HttpExtensions
  [<System.Runtime.CompilerServices.Extension>]   
  let AddBearer(this : System.Net.WebClient, creds: Credentials) =
    this.AddBearer(creds)

[<AutoOpen>]
[<System.Runtime.CompilerServices.Extension>]
module HttpWebRequestExtensions = 
  open HttpExtensions
  [<System.Runtime.CompilerServices.Extension>]   
  let AddBearer(this : System.Net.HttpWebRequest, creds: Credentials) =
    this.AddBearer(creds)

[<AutoOpen>]
[<System.Runtime.CompilerServices.Extension>]
module HttpWebRequestMethodExtensions = 
  open HttpExtensions
  [<System.Runtime.CompilerServices.Extension>]   
  let AddBearer(this : System.Net.Http.HttpRequestMessage, creds: Credentials) =
    this.AddBearer(creds)



