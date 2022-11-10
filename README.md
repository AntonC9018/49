## Setup:


1. Install .NET6 (the build script might be doing it on its own too);
2. Install NodeJS;
3. Run the build script in the root folder, which will install nuke and try to do a build.


To build for production, do `build publish` or `nuke publish`, which calls the same nuke target.
The output will be in the `output/app` directory.

For development:
- Start up the vite server in the folder `source/fourtynine.ClientApp` with `npm run dev`;
- Start up the ASP.NET backend with hot reloading in `source/fourtynine` with `dotnet watch`.


Tech used:

- [Vite](https://vitejs.dev/) for package management and maybe later some React pages;
- [Tailwind](https://tailwindcss.com/) for styling;
- [YARP](https://microsoft.github.io/reverse-proxy/index.html) to forward asset requests to the Vite server in development.

 