﻿## Setup:


1. Install .NET6 (the build script might be doing it on its own too);
2. Install [NodeJS 18+](https://nodejs.org/en/);
3. Install [SqlServer 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads);
4. Run the build script in the root folder, which will install [Nuke](https://nuke.build/) and try to do a build.


To build for production, do `build publish` (or `nuke publish`, which calls the same nuke target).
The output will be in the `output/app` directory.

For development:
- Call `build startdev`, which in turn does the following:
    - Starts up the vite server in the folder `source/fourtynine.ClientApp` with `npm run dev`;
    - Starts up the ASP.NET backend with hot reloading in `source/fourtynine` with `dotnet watch`.

Assuming the database client is installed on your machine, the database should get created automatically.

If you make changes to the API, the TypeScript client has to be regenerated manually. For that, run `build generateSwaggerTypeScriptClient` while the backend is running.

Tech used:

- [Vite](https://vitejs.dev/) for package management and maybe later some React pages;

- [Tailwind](https://tailwindcss.com/) for styling;

- I've tried using [YARP](https://microsoft.github.io/reverse-proxy/index.html) to forward asset requests to the Vite server in development.
This didn't quite work, since the YARP proxy always takes priority over API controllers, even though the developer claims the opposite.
See [my comment in discussions](https://github.com/microsoft/reverse-proxy/discussions/792#discussioncomment-4119355), which has received no answer at the time of writing this.
I may want to open an issue at some point.

- For proxies I'm using [ASP.NET Core Proxy](https://github.com/twitchax/AspNetCore.Proxy).
The API is arguably worse, because it works with strings for uris, so it doesn't take advantage of the type system.
It fails to perform common checks, and produces warnings in Kestrel. See [my issue](https://github.com/twitchax/AspNetCore.Proxy/issues/101).
I'm getting the general impression that proxy API's in ASP.NET Core are quirky and buggy in practice.

- Another option I've looked at for proxying was the Microsoft's SPA and SPA Extensions packages, but they're no longer maintained, in favor of YARP, I suppose (YARP is maintained by Microsoft).

- [Nuke](https://nuke.build/) for build automation and script management (I'm not quite sure how good it is for scripts, might be better off doing a custom tool with some references to it instead);

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) with [SqlServer](https://docs.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli) for database access;

- [NSwag](https://github.com/RicoSuter/NSwag) for generating the TypeScript API client;

- [AutoMapper](https://automapper.org/).
Tried mapping manually initially, but it's very clear to me that that approach is unmaintainable, resulting in a lot of code duplication, or complications from combining expression trees.
At that point it's clearly worth it to use a library.

- I needed to validate the used form names, so that they always correspond to the expected DTO.
Since we've got NSwag, TypeScript has information on the types through the generated client, so I decided to dig into [TypeScript reflection](https://github.com/Hookyns/tst-reflect) to do said validation.
I'm now thinking it's better to validate through the swagger spec.
Either checks should be disabled in production though.
After some thought and experimentation, I went ahead and did a hack to generate the reflection info that I need for validation using NSwag in the client generation Nuke task. The reflection did not work out, see [my issue](https://github.com/Hookyns/tst-reflect/issues/83).