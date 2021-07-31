The application is build on the following technology stack:

Blazor (see https://blazor.net) as front end based on the WebAssembly (see https://webassembly.org) standard, supported by all modern browsers (desktop and mobile). That means that the client is an application running in the browser as SPA (Single Page Application), sandboxed like JavaScript.
ASP.NET Core Backend (see https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.1#blazor-webassembly)
Business Logic in Azure Functions v3 (see https://docs.microsoft.com/en-us/azure/azure-functions/)
Data Layer No-SQL database Azure Cosmos DB (see https://docs.microsoft.com/en-us/azure/cosmos-db/)
To get real-time updates for MeetUps SignalR (see https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr-blazor-webassembly) is used, inspired by the example from https://www.c-sharpcorner.com/article/easily-create-a-real-time-application-with-blazor-and-signalr/
Some helpers: Blazored is an excellent colletion with some tools to make the life easier with Blazor. MeetUpPlanner uses the TextEdit and LocalStorage components.

The Blazor Component Library from Radzen provides a comprehensive set of components used by MeetUpPlanner for notification messages and numeric input.

Another handy tool to provide file download in Blazor: https://github.com/arivera12/BlazorDownloadFile

And of course Bootstrap for the responsive design is used.

To assemble link previews LinkTools https://github.com/MSiccDev/LinkTools is used: Copyright (c) 2021 MSiccDev Software Development (Marco Siccardi)
