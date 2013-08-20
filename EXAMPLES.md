How to run the example projects
===============================

In order to run either of the two example projects, `ExampleMemberList` or `ExampleMemberListCSharp`, you'll need to follow the standard steps for permitting an OAuth application in the instance of VersionOne you want to use.

Configurations steps
--------------------

We'll assume you have VersionOne installed locally at `http://localhost/VersionOne.Web`. If you want to run against a different location, use that URI insead.

The configuration process is the same for either of the examples, so we'll use `ExampleMemberListCSharp`.

* Compile the `ExampleMemberListCSharp` and `GrantTool` projects in Visual Studio.
* If everything compiled correctly, start following the steps in [Using OAuth 2.0 for Installed Applications]
(https://community.versionone.com/Developers/Developer-Library/Documentation/API/Security/Oauth_2.0_Authentication/Using_OAuth_2.0_for_Installed_Applications), from the VersionOne Community site.
* At the `Add Permitted App` step, use `Example Member List` for the `Name` field.
* After clicking `Add Application`, click the `Download JSON` link and save the `client_secrets.json` file it prompts you with in the `GrantTool\bin\Debug` folder.
    * **Note:** In your `clients_secrets.json` file, the `auth_uri` and `token_uri` must be fully-qualified, like  `http://localhost/VersionOne.Web/oauth.v1/auth`, and not just `/VersionOne.Web.oauth.v1/auth`. You can go to http://localhost/VersionOne.Web/Default.aspx?menu=ConfigureSMTPPage and set the `URL Prefix` value to `http://localhost` in order to get the Permitted Apps page to include it by default.
* Open up a command prompt and navigate to the `GrantTool\bin\Debug` folder and type `granttool apiv1 client_secrets.json`
    * This will give a URL that you need to open in your browser to authorize the `Example Member List` application to use the `apiv1` feature of the target VersionOne instance.
* Once you authorize the request, copy and paste the value back into the command line and hit enter.
    * The grant tool creates `stored_credentials.json` in the same folder.
* **Finally, copy both `client_secrets.json` and `stored_credentials.json` to the `ExampleMemberListCSharp\bin\Debug` folder.**
* Set `ExampleMemberListCSharp` as the startup project, and then run it!
* You should see a list of `Member` assets in JSON format, like this:

```json
 {
   "_type" : "Asset",
   "href" : "/VersionOne.Web/rest-1.oauth.v1/Data/Member/1008",
   "id" : "Member:1008",
   "Attributes" : {
     "AssetType" : {
       "_type" : "Attribute",
       "name" : "AssetType",
       "value" : "Member"
     },
     "IsCollaborator" : {
       "_type" : "Attribute",
       "name" : "IsCollaborator",
       "value" : false
     },
     "Email" : {
       "_type" : "Attribute",
       "name" : "Email",
       "value" : "joe@company.com"
     },
     "Nickname" : {
       "_type" : "Attribute",
       "name" : "Nickname",
       "value" : "Joe"
     },
     "Description" : {
       "_type" : "Attribute",
       "name" : "Description",
       "value" : null
     },
     "Name" : {
       "_type" : "Attribute",
       "name" : "Name",
       "value" : "Joe IT"
     },
     "AssetState" : {
       "_type" : "Attribute",
       "name" : "AssetState",
       "value" : 128
     },
     "Phone" : {
       "_type" : "Attribute",
       "name" : "Phone",
       "value" : "800.555.1220"
     },
     "DefaultRole" : {
       "_type" : "Relation",
       "name" : "DefaultRole",
       "value" : {
         "_type" : "Asset",
         "href" : "/VersionOne.Web/rest-1.oauth.v1/Data/Role/3",
         "idref" : "Role:3"
       }
     },
     "Username" : {
       "_type" : "Attribute",
       "name" : "Username",
       "value" : "joe"
     },
     "IsLoginDisabled" : {
       "_type" : "Attribute",
       "name" : "IsLoginDisabled",
       "value" : true
     },
     "SecurityScope" : {
       "_type" : "Relation",
       "name" : "SecurityScope",
       "value" : null
     },
     "NotifyViaEmail" : {
       "_type" : "Attribute",
       "name" : "NotifyViaEmail",
       "value" : false
     },
     "UsesLicense" : {
       "_type" : "Attribute",
       "name" : "UsesLicense",
       "value" : false
     },
     "SendConversationEmails" : {
       "_type" : "Attribute",
       "name" : "SendConversationEmails",
       "value" : true
     },
     "Avatar" : {
       "_type" : "Relation",
       "name" : "Avatar",
       "value" : null
     },
     "DefaultRole.Name" : {
       "_type" : "Attribute",
       "name" : "DefaultRole.Name",
       "value" : "Role.Name'Project Lead"
     },
     "SecurityScope.Name" : {
       "_type" : "Attribute",
       "name" : "SecurityScope.Name",
       "value" : null
     },
     "Ideas" : {
       "_type" : "Attribute",
       "name" : "Ideas",
       "value" : []
     },
     "Followers" : {
       "_type" : "Relation",
       "name" : "Followers",
       "value" : []
     },
     "Followers.Name" : {
       "_type" : "Attribute",
       "name" : "Followers.Name",
       "value" : []
     },
     "Followers.Nickname" : {
       "_type" : "Attribute",
       "name" : "Followers.Nickname",
       "value" : []
     }
   }
 }

```



