# MeetUp-Planner
Everything you need to organize and track a "MeetUp", e.g. organizing a group bike-ride. 

The first idea for this application came in the "Corona" times: In order to arrange group bike rides or other "MeetUps" 
and at the same time to ensure the traceability of the participants, the corresponding tool was missing. 

Design principles are:
* Easy to use without complicated registration and authentication process. Therefore only "keywords" are used as very basic access control.
* Responsive design, usable on PC and smartphones
* Minimal administrative effort: All meetups data should be deleted automatically after the configured time (typically 28 days). Everyone should be able to organize a MeetUp.
* Availability of "private" MeetUps: The ability to organize a MeetUp and protect it with a "keyword" that can be given to potential participants. 
* Adaptable: The application has a front-end layer implemented with <a href="https://docs.microsoft.com/en-us/aspnet/core/blazor">ASP.NET Core Blazor</a>  The application logic is provided as microservices (Azure Functions). In this way it is possible to use the application layer independently with a different front-end.  

About this repository: Folder "MeetUpFunctions" has the source code of the Azure Functions used for the backend. The master branch is CI enabled with GitHub Actions and deployed to the slot "dev" of the Azure Functions App. 
Folder MeetUpPlanner holds the source code of the Blazor WebAssembly and the ASP.NET Core hosting app. The master branch is CI enabled with GitHub Action and deployed to https://meetup-planner-dev.azurewebsites.net

For documentation including deployment instructions see Wiki
