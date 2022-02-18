using Microsoft.Identity.Client;

IPublicClientApplication app = PublicClientApplicationBuilder
    .Create("28ecd1c7-059d-40a4-86b9-2f04ab4f9178")
    .WithAuthority("https://login.microsoftonline.com/common/v2.0")
    .WithRedirectUri("http://localhost:8888")
    .Build();

var bld = app.AcquireTokenInteractive(new string[] {"api://28ecd1c7-059d-40a4-86b9-2f04ab4f9178/Access"});
var result = await bld.ExecuteAsync();
System.Console.WriteLine(result.AccessToken);