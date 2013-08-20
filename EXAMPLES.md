How to run the example projects
===============================

In order to run either of the two example projects, `ExampleMemberList` or `ExampleMemberListCSharp`, you'll need to follow the standard steps for permitting an OAuth application in the instance of VersionOne that you want to run the example against. 

Configurations steps
--------------------

We'll assume you have VersionOne installed locally at `http://localhost/VersionOne.Web`. If you want to run against a different location, use that URI insead.

The configuration process is the same for either of the examples, so we'll use `ExampleMemberListCSharp`.

* Compile the `ExampleMemberListCSharp` project in Visual Studio
* If everything compiled correctly, start following the steps in [Using OAuth 2.0 for Installed Applications]
(https://community.versionone.com/Developers/Developer-Library/Documentation/API/Security/Oauth_2.0_Authentication/Using_OAuth_2.0_for_Installed_Applications), from the VersionOne Community site
* At the `Add Permitted App' step, use `Example Member List` for the `Name` field
* After clicking `Add Application`, click the `Download JSON` link and save the `client_secrets.json` file it prompts you with in the `bin\Debug` folder of your `ExampleMemberListCSharp` project
    * **Note:** In your `clients_secrets.json` file, the `auth_uri` and `token_uri` but be fully-qualified, like  `http://localhost/VersionOne.Web/oauth.v1/auth`, and not just `/VersionOne.Web.oauth.v1/auth`. You can go to http://localhost/VersionOne.Web/Default.aspx?menu=ConfigureSMTPPage and set the `URL Prefix` value to `http://localhost` in order to get the Permitted Apps page to include it by default.


