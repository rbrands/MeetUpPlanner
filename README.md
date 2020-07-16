# MeetUp-Planner
Everything you need to organize and track a "MeetUp" 

The first idea for this application came in the "Corona" times: In order to arrange group bike rides or other "MeetUps" 
and at the same time to ensure the traceability of the participants, the corresponding tool was missing. 

Design principles are:
* Easy to use without complicated registration and authentication process. Therefore only "keywords" are used as very basic access control.
* Responsive design, usable on PC and smartphones
* Minimal administrative effort: All meetups data should be deleted automatically after the configured time (typically 28 days)
* Adaptable: The application has a front-end layer implemented with <a href="https://docs.microsoft.com/en-us/aspnet/core/blazor">ASP.NET Core Blazor</a>  The application logic is provided as microservices (Azure Functions). In this way it is possible to use the application layer independently with a different front-end.  

