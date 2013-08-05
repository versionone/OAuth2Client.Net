namespace OAuth2Client

open System.Threading.Tasks

type IStorage = 
  abstract member GetSecrets : unit -> Task<Secrets>
  abstract member GetCredentials : unit -> Task<Credentials>
  abstract member StoreCredentials : Credentials -> Task<Credentials>
  

