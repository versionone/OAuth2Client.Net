module OAuth2ClientTests.ClientSecretsTests

open NUnit.Framework

open OAuth2Client

let v1Json = """
  {
    "installed": {
      "client_id": "client_nbcnzaro",
      "client_name": "Yaml Client Test",
      "client_secret": "s2esvwh682aec2oj6bqs",
      "redirect_uris": [
        "urn:ietf:wg:oauth:2.0:oob"
      ],
      "auth_uri": "http://localhost/VersionOne.Web/oauth.mvc/auth",
      "token_uri": "http://localhost/VersionOne.Web/oauth.mvc/token",
      "server_base_uri": "http://localhost/VersionOne.Web",
      "expires_on": "9999-12-31T23:59:59.9999999"
    }
  }
  """

let googleJson = """
  {
    "installed": {
      "client_id": "client_nbcnzaro",
      "client_secret": "s2esvwh682aec2oj6bqs",
      "redirect_uris": [
        "urn:ietf:wg:oauth:2.0:oob"
      ],
      "auth_uri": "http://localhost/VersionOne.Web/oauth.mvc/auth",
      "token_uri": "http://localhost/VersionOne.Web/oauth.mvc/token"
    }
  }
  """

let expectedSecrets = {
  client_type = Installed
  client_id = "client_nbcnzaro"
  client_secret = "s2esvwh682aec2oj6bqs"
  auth_uri = System.Uri("http://localhost/VersionOne.Web/oauth.mvc/auth")
  token_uri = System.Uri("http://localhost/VersionOne.Web/oauth.mvc/token")
  redirect_uris = [System.Uri("urn:ietf:wg:oauth:2.0:oob")]
  }

let [<Test>] ``parses a valid client_secrets.json from V1`` () = 
  let secrets = Secrets.FromJson v1Json
  Assert.AreEqual(expectedSecrets, secrets)

let [<Test>] ``parses a valid client_secrets.json from Google`` () = 
  let secrets = Secrets.FromJson googleJson
  Assert.AreEqual(expectedSecrets, secrets)