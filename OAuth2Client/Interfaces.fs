namespace OAuth2Client

open System.Threading.Tasks

type IStorageAsync = 
  abstract member GetSecretsAsync : unit -> Task<Secrets>
  abstract member GetCredentialsAsync : unit -> Task<Credentials>
  abstract member StoreCredentialsAsync : Credentials -> Task<Credentials>
  
type IStorage =
  abstract member GetSecrets : unit -> Secrets
  abstract member GetCredentials : unit -> Credentials
  abstract member StoreCredentials : Credentials -> Credentials
  
