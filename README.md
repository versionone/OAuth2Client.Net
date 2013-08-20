OAuth2Client.Net
================

Basic OAuth2 client for .NET providing an easy way to authenticate to OAuth2-protected endpoints.


What It Does
------------

  * Does allow your service to simply act as a client and authenticate against VersionOne or another OAuth2 host.

  * Does use simple JSON credential storage, modeled after the google oauth2 python library.

  * Does allow you to customize the storage implementation, but comes with a reasonable default.

  * Does provide an extension to System.Net.Http.HttpClient that gives you a ready-for-OAuth2 HttpClient.

  * Does provide both synchronous and asynchronous methods, in case you can't use async.

  * Does support the Out-Of-Band flow for services that don't have a public listening HTTP endpoint.


What It Doesn't
---------------

  * Does not require you to be running inside an IIS or any other webserver

  * Does not open or use a listening HTTP endpoint

  * Does not require your code to have any kind of user-visible UI.

  * Does not want to pass your user through to Facebook or Google or Twitter for "identity"

  * Does not require implementation of lots of interfaces or mixing a dozen methods into a predefined flow.




Getting started
===============

* Add the OAuth2Client and VersionOne.SDK.APIClient.Net dependencies to your project via NuGet

* Create a client or app identity on the host that we'll be authenticating to.

* download the client_secrets.json file from the host, or construct one by hand.

* run the command-line tool to generate a grant URL, and follow that url to allow the permissions

* copy and paste the credentials into the command line tool, producing a stored_credentials.json file

* put the resulting client_secrets.json and stored_credentials.json file where the client code can find them

  * Either in the CWD of the running code

  * or specified by the environment variables OAUTH2_SECRETS / OAUTH2_CREDS

  * or loaded by your implementation of OAuth2Client.IStorage which will return OAuth2Client.Secrets and OAuth2Client.Credentials objects.



* instantiate a V1OAuth2APIConnector

  * Provide an OAuth2Client.IStorage instead of any username/password details

  * The connector will load the credentials from the Storage and will apply them to outbound requests

  * if a request fails due to 401, it will refresh the token and retry the request once.

    * if the refresh fails (e.g. the app entry has been suspended or deleted), the refresh will fail.


* Use the V1OAuth2APIConnector where you would use a V1APIConnector




