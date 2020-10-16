[https://www.meetupplanner.de](https://www.meetupplanner.de)
# Ersteinrichtung für eine Radgruppe

Um den MeetUpPlanner für eine Radgruppe erstmalig einzurichten, sind die folgenden Schritte notwendig:
- Festlegung der Adresse im Web: Der MeetUpPlanner muss über eine eindeutige Adresse erreichbar sein, also z.B. meinclub.meetupplanner.de. Dies muss im Vorfeld geklärt werden. Die Adresse wird im Programm-Code einmalig festgelegt, daher wird hier etwas Vorlaufzeit benötigt. 
- Soll der MeetUpPlanner über eine eigene Domain erreichbar sein, also z.B. ausfahrten.meinclub.de muss der Domain-Inhaber einen sog. CNAME-Record und einen TXT Record anlegen. 
  - CNAME ausfahrten.meinclub.de auf die Adresse, die gesondert mitgeteilt wird.
  - TXT asuid.ausfahrten.meinclub.de mit der ASUID, die gesondert mitgeteilt wird.
  - Das Verfahren wird auch [hier](https://docs.microsoft.com/de-de/azure/app-service/app-service-web-tutorial-custom-domain#cname) genauer erklärt.
- Grundsätzliche Einstellung: Soll es ein Feld "Vereinsmitglied?" geben? Falls ja, können Teilnehmer dies ankreuzen und brauchen keine weitere Adressinfo einzugeben. Das passt nur bei Vereinen, die vollständige Mitgliederverzeichnisse mit Adressen haben.
- Danach ist der MeetUpPlanner und der oben vereinbarten Adresse erreichbar und der Admin sollte die folgenden grundlegenden Einstellungen in der Administration vornehmen:
  - Titel: Steht oben links in der App also z.B. "Mein Super Club"
  - Optional: Link und Titel des Links. Steht unten links in der App
  - Max. Gruppengröße
  - Max. Gruppengröße für Admins
  - Anzahl der Buchstaben in der Anzeige der Nachnamen oder 0 falls die Nachnamen komplett angezeigt werden sollen.
  - Einladen von Gästen möglich? Wenn angekreuzt, können externe Gäste (also ohne Schlüsselwort) über die entsprechenden Links eingeladen werden.
  - Sollen nur Admins MeetUps anlegen können? Wenn angekreuzt können nur Admins Ausfahrten anlegen, alle anderen können nur teilnehmen.
  - Optional: Welcome Nachricht: Dies ist die Nachricht, die die Benutzer auf der Startseite sehen. 
  - Optional: Link zu einem Logo, wird auch auf der Startseite angezeigt.
  - Optional: Whiteboard-Nachricht wird auf der Kalendar-Seite angezeigt. Hier könnten ggf. noch Hinweise zu den "Spielregeln" stehen. Aber besser hier kurz fassen, weil ansonsten alle immer erst einmal runter scrollen müssen bis endlich die Ausfahrten erscheinen ..
   
  
