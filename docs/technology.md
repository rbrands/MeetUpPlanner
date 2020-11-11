# Zur Technik

Der MeetUpPlanner ist eine Web-Anwendung, allerdings in einer besonderen "Spielart": Es handelt sich um eine sog. ["Single-Page Application" (SPA)](https://de.wikipedia.org/wiki/Single-Page-Webanwendung), die zunächst vom Server runter geladen wird (daher kommt die Meldung "Momentchen" zu Beginn) und danach im Browser lokal auf dem Handy/Rechner ausgeführt wird. Dadurch fühlt sie sich eher wie eine App als eine Web-Anwendung an. Die Daten werden dann immer jeweils vom Server aktualisiert. Außerdem wird hier der sog. [WebAssembly Standard](https://de.wikipedia.org/wiki/WebAssembly) genutzt, dadurch wird es möglich, dass das auf allen modernen Browsern funktioniert. Von so etwas hat man vor Jahren geträumt ...

Mittlerweile ist der MeetUpPlanner auch eine ["Progressive Web App (PWA)"](https://de.wikipedia.org/wiki/Progressive_Web_App), d.h. er lässt sich aus dem Browser heraus als App installieren. Außerdem kann er Push-Benachrichtigungen senden, z.B. bei neuen Kommentaren zu einer Ausfahrt oder falls eine Ausfahrt abgesagt wird.

Für die Entwicklung von SPAs verwendet man üblicherweise etablierte Frameworks wie [Angular](https://de.wikipedia.org/wiki/Angular) oder [React](https://de.wikipedia.org/wiki/React). Für den MeetUpPlanner wurde allerdings das relativ neue [ASP.NET Core Blazor](https://docs.microsoft.com/de-de/aspnet/core/blazor) verwendet.

Auf der Serverseite wurde die Business-Logik "serverless" mit [Azure Functions](https://docs.microsoft.com/de-de/azure/azure-functions/) umgesetzt. Als Datenbank wird die No-SQL Datenbank [Cosmos DB](https://docs.microsoft.com/de-de/azure/cosmos-db/). Für weitere Einzelheiten siehe das GitHub Repository.

Und schließlich: Die Serverseite der Anwendung läuft in Microsoft's Azure Cloud.

Aus Security-Gründen wurde die Administration der Anwendung - also Pflege der Einstellungen, Export der Kontaktliste - in eine separate Anwendung verlagert, um hier durch Authentifizierung und nicht nur durch ein "Schlüsselwort" den Zugriff zu kontrollieren. Diese Anwendung ist auch auf GitHub unter [https://github.com/rbrands/AdminMeetUpPlanner](https://github.com/rbrands/AdminMeetUpPlanner) verfügbar. Diese Anwendung ist auch eine Blazor-Anwendung, die mit den neuen [Azure Static Webs](https://azure.microsoft.com/en-us/services/app-service/static/#overview) realisiert wurde. 
