## Setup:


1. Install .NET6 (the build script might be doing it on its own too);
2. Install [NodeJS 18+](https://nodejs.org/en/);
3. Install [SqlServer 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads);
4. Run the build script in the root folder, which will install [Nuke](https://nuke.build/) and try to do a build.
5. You need to set all secrets using `dotnet user-secrets` for in the folder `source/fourtynine` (see notes on why this is not shared automatically in the notes below):
  ```
  dotnet user-secrets OAuthGithubClientId <github client id>
  dotnet user-secrets OAuthGithubClientSecret <github client secret>
  ```

To build for production, do `build publish` (or `nuke publish`, which calls the same nuke target).
The output will be in the `output/app` directory.

For development:
- Call `build startdev`, which in turn does the following:
    - Starts up the vite server in the folder `source/fourtynine.ClientApp` with `npm run dev`;
    - Starts up the ASP.NET backend with hot reloading in `source/fourtynine` with `dotnet watch`.

Assuming the database client is installed on your machine, the database should get created automatically.

If you make changes to the API, the TypeScript client has to be regenerated manually. For that, run `build generateSwaggerTypeScriptClient` while the backend is running.

## Notes

- [Vite](https://vitejs.dev/) has been used for package management and maybe later some React pages;

- [Tailwind](https://tailwindcss.com/) has been used for styling;

- I've tried using [YARP](https://microsoft.github.io/reverse-proxy/index.html) to forward asset requests to the Vite server in development.
This didn't quite work, since the YARP proxy always takes priority over API controllers, even though the developer claims the opposite.
See [my comment in discussions](https://github.com/microsoft/reverse-proxy/discussions/792#discussioncomment-4119355), which has received no answer at the time of writing this.
I may want to open an issue at some point.

- For proxies I'm using [ASP.NET Core Proxy](https://github.com/twitchax/AspNetCore.Proxy).
The API is arguably worse, because it works with strings for uris, so it doesn't take advantage of the type system.
It fails to perform common checks, and produces warnings in Kestrel. See [my issue](https://github.com/twitchax/AspNetCore.Proxy/issues/101).
I'm getting the general impression that proxy API's in ASP.NET Core are quirky and buggy in practice.

- Another option I've looked at for proxying was the Microsoft's SPA and SPA Extensions packages, but they're no longer maintained, in favor of YARP, I suppose (YARP is maintained by Microsoft).

- [Nuke](https://nuke.build/) has been used for build automation and script management (I'm not quite sure how good it is for scripts, might be better off doing a custom tool with some references to it instead);

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) with [SqlServer](https://docs.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli) has been used for database access;

- [NSwag](https://github.com/RicoSuter/NSwag) has been used for generating the TypeScript API client;

- [AutoMapper](https://automapper.org/).
Tried mapping manually initially, but it's very clear to me that that approach is unmaintainable, resulting in a lot of code duplication, or complications from combining expression trees.
At that point it's clearly worth it to use a library.

- I needed to validate the used form names, so that they always correspond to the expected DTO.
Since we've got NSwag, TypeScript has information on the types through the generated client, so I decided to dig into [TypeScript reflection](https://github.com/Hookyns/tst-reflect) to do said validation.
I'm now thinking it's better to validate through the swagger spec.
Either checks should be disabled in production though.
After some thought and experimentation, I went ahead and did a hack to generate the reflection info that I need for validation using NSwag in the client generation Nuke task. The reflection did not work out, see [my issue](https://github.com/Hookyns/tst-reflect/issues/83).

- I've had a lot of trouble trying to make the razor pages code DRY.
I thought tag helpers would be good for this, but the thing is, while they can render partials, those partials can not pass `ModelExpression`'s as parameters to other tag helpers.
See e.g. [this stackoverflow answer](https://stackoverflow.com/a/55474543/9731532), which I don't understand in the slightest.
It involves some really tricky magic and introduces a lot of boilerplate just to juggle the contexts.
So I decided to ditch the tag helpers and use html helpers, specifically editor/display templates. These just use html helpers to achieve the same effect. Rendering these inside partials still involves some magical context juggling, but it's far less annoying. The documentation is sparse too.
See [this blog](https://cpratt.co/displaytemplates-and-editortemplates-for-fun-and-profit/) for some enlightenment on the topic.

I wanted to implement authentication using services like GitHub or Google.
These require you to register an application, and then use the secret you get from them to use in the OpenIDConnect or OAuth2 flows.
The existence of secrets means that they have to be shared or recreated on developer machines, but checking them in with the source code is obviously a bad practice.
This calls for a cloud service for sharing secrets.
I've landed on [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/), because it's free with my student pack license.

The three things I wanted to be able to do were the following:
- Be able to access the secrets from any machine, as long as I'm logged in as myself;
- Be able to grant or revoke access to the vault to specific users;
- If I'm not authenticated on a new machine, I want an automated log in process, possibly in the restore Nuke task.

Things aren't so simple though.
Since my credit is tied with the active directory of my university, I won't be able to give access to the vault to users outside my university (I think).
I can create a new active directory, but that won't have the credit anymore.
I'm sure in a real company on a real project this would not be a problem.

Also, there are multiple authentication concepts for Azure Key Vault, not just Azure Active Directory, but also Managed Identity, RBAC, which I have not worked with and hence don't know which one I need to use if any.

So, for the sake of progress, I'll fall back to storing the secrets locally with [the secret manager tool](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#how-the-secret-manager-tool-works), so the secrets will either have to be shared on to the new machines, or generated again.