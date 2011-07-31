This is a simple implementation of git-http-backend written in ASP.NET 
that can be used to read/write git repositories on Windows with IIS, or Linux with mono xsp or apache.

Inspired by Grack (http://github.com/schacon/grack)

This is largely untested, but has been developed with IIS7.5 under Windows 7 x64. 

The version of GitSharp included is a custom build with some minor changes and bug fixes.
Current GitSharp.dll version is 0.3.99.0 (linquize)
Current GitSharp.Core.dll version is 0.3.99.2 (linquize)

Requirements:
- Development tools: Visual Web Developer 2010 with .NET 4
- ASP.NET: MVC3 (Razor)
- Windows: IIS5.1+ (for IIS5.1, add application settings: extension=*, path=c:\windows\microsoft.net\framework\v4.0.30319\aspnet_isapi.dll)
- Linux: Mono 2.10.2, xsp, use xsp standalone or apache

Edit the web.config and change the "RepositoriesDirectory" app-setting to point to a directory containing git repositories.

Assuming that your repositories directory looks like this:

C:\Repositories\Repo1.git

...and the RepositoriesDirectory app-setting is configured to be C:\Repositories:

<appSettings>
		<add key="RepositoriesDirectory" value="C:\Repositories" />
		<add key="RepositoryLevel" value="1" />
</appSettings>
	
...and the application is configured under IIS7 on port 8000, then issuing the following command will cone the Repo1.git repository:

git clone http://localhost:8000/Repo1.git

Once cloned, push/pull work as expected.

This fork includes a file viewer and file download function.
If your source code contains *.cs, *.csproj, *.vb, *.vbproj, *.java, web.config, bin directory, etc...,
please remove those extensions and "Hidden Segments" you need in IIS "Request Filtering".

There are currently no tests (something I hope to rectify soon). If you run into a problem, the best way to troubleshoot is by using Fiddler to see the raw request/response data.