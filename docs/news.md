[https://www.meetupplanner.de](https://www.meetupplanner.de)
# Änderungshistorie zum MeetUpPlanner
*7.2.2021*
- Für MeetUps kann jetzt angegeben werden, dass kein (oder 1) Teilnehmer möglich ist. Das ist für die Scuderia Winkerunde gedacht, bei der die Fahrer zwar die gleiche Strecke fahren, aber nicht zusammen.

*12.12.2020*
- Umstellung auf .NET 5. Siehe dazu [https://devblogs.microsoft.com/dotnet/announcing-net-5-0/](https://devblogs.microsoft.com/dotnet/announcing-net-5-0/). Hier wichtig sind die Performanceverbesserungen, aber vor allem neue Feature wie die FileInput-Komponente, die demnächst benötigt wird ([https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-5-release-candidate-1/](https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-5-release-candidate-1/))

*10.11.2020*
- "Private Schlüsselwörter" wurden bei Anzeige der Termine für Gäste angezeigt. Diese werden jetzt hier ausgeblendet.
- Bei Push-Nachrichten wird jetzt der Titel einer Ausfahrt als Überschrift verwendet.
- Interne Änderung: Die Konfiguration der Mandanten erfolgt jetzt nicht mehr über eine Tabelle im Programm-Code sondern in der Datenbank. Dafür gibt es ein eigenes Tool unter https://admin.meetupplanner.de
- Aus Security-Gründen erfolgt die Administration nicht mehr im MeetUpPlanner selbst sondern im Admin-Tool https://admin.meetupplanner.de Der Hintergrund für diese Änderung ist, dass die "Schlüsselwörter" und hier insbesondere das Admin-Schlüsselwort keine richtige Authentifizierung ersetzen und persönliche Daten - insbesondere in der Kontaktliste, die im Fall einer Covid-19 Erkrankung exportiert werden kann und soll - so unzureichend geschützt sind. Das ist auch nicht im Sinne der DSGVO. Das Admin-Tool erfordert eine Authentifizierung (via Microsoft Account, Google Account, Facebook, Twitter oder GitHub) und die Zuweisung entsprechender Berechtigungen. Dann können dort die Einstellungen, die bisher unter "Administration" erreichbar waren, erfolgen. Das gleiche gilt für den Export von Kontakten. Ansonsten hat dies keine Auswirkungen für die Benutzer, d.h. es bleibt beim allgemeinen "Schlüsselwort" zur Erstellung von Ausfahrten und zur Anmeldung. Das "Admin-Schlüsselwort" behält seine erweiterten Rechte was Ausfahrten angeht, also regelmößige Ausfahrten, Ausfahrten ohne Guide, höher Teilnehmerzahl usw. 

*28.10.2020*
- Push-Benachrichtigung: Man kann sich jetzt für Ausfahrten, an denen man teilnimmt, über Änderungen und Kommentare benachrichtigen lassen. iOS (also iPhone usw.) unterstützt diese "Web Pushnachrichten" noch nicht, da muss also darauf verzichtet werden
- Wird der MeetUpPlanner gesperrt (über die Einstellungen in "Administration") konnte auf die Gäste-Seiten weiterhin zugegriffen werden. Diese "Lücke" ist jetzt gestopft, d.h. auch diese Seiten sind gesperrt.

*24.10.2020*
- Der MeetUpPlanner ist jetzt eine PWA (["Progressive Web Application"](https://de.wikipedia.org/wiki/Progressive_Web_App)). Eine PWA lässt sich vom Browser aus wie eine App installieren. Vorteil: Die App ist schneller aktiv und bleibt auch länger aktiv. Hinweise zur Installation im Userguide. Aber vor allem ist dies die Voraussetzung für das nächste Feature, das implementiert wird: Push-Benachrichtungen, für Absagen von Ausfahrten, Kommentaren usw.
- Explizite Fehlermeldung wenn keine Netzverbindung da ist.

*18.10.2020*
- Beim Wechsel von MeetUps von "mit Guide" zu "ohne Guide" und umgekehrt gab es einige Fehler und Unklarheiten. Jetzt ist es folgendermaßen umgesetzt: Wird eine Ausfahrt, die bereits einen Guide hat, zu "ohne Guide" umgewandelt, wird der bisherige Guide als Teilnehmer hinzugefügt. Er/sie muss sich dann in einem extra Schritt abmelden, wenn das nicht gewünscht wird. Wird eine Ausfahrt "ohne Guide" umgewandelt in eine "mit Guide" wird der aktuelle Benutzer (der ja Admin ist) als Guide eingetragen und ggf. als Teilnehmer gelöscht, falls vorher schon eingetragen. Falls ein anderer Guide gewünscht wird, muss im nächsten Schritt ein neuer Guide zugewiesen werden.
Außerdem wird in der Kontaktliste jetzt nicht mehr fälschlicherweise der ursprüngliche Guide mit aufgeführt, wenn die Ausfahrt in "ohne Guide" geändert wird.
- Wird ein Meeting abgesagt, wird kurz eine Meldung angezeigt, dass man durch erneutes Drücken des Buttons die Absage auch wieder rückgängig machen kann.
- Kleine Darstellungsänderung: In der Kalendaransicht die Spalten für "große Bildschirme" (also PCs) breiter gemacht.
- Neuer "Mandant" flannelspandex

*15.10.2020*
- Das "WTT"-Feature: Für Gäste gab es bisher nur die Möglichkeit für genau eine Ausfahrt einen Link zu teilen. Jetzt gibt es eine weitere Möglichkeit: Für Ausfahrten kann optional ein "Scope" wie z.B. "WTT" angegeben werden. Über den Link ../guests/WTT können dann für Gäste alle Ausfahrten angezeigt werden, die so gekennzeichnet wurde. Dieser Link kann dann den potentiellen Mitfahrern z.B. über einen Strava-Club bereit gestellt werden, ohne immer einen neuen Link generieren zu müssen.
- Auf der Seite mit der Anzeige der Ausfahrten, gibt es jetzt einen Hilfe-Button, der standardmäßig auf [https://www.meetupplanner.de/userguide](https://www.meetupplanner.de/userguide) verlinkt ist. Dieser Link kann in den Einstellungen geändert werden auf eine für den Club spezifische Seite.
- Darstellung gefixed: Die Buttons auf der Kalendarseite sind jetzt gruppiert mit einem möglichen Zeilenumbruch.

*7.10.2020*
- Ausfahrten können als "Abgesagt!" markiert werden. Im Titel wird dann ein entsprechender "Badge" zur Kennzeichnung angezeigt und die Anmeldung ist nicht mehr möglich. Abmeldung schon. Ausfahrten die als abgesagt gekennzeichnet werden, werden in der Kontaktliste nicht berücksichtigt.
- Es kann jetzt konfiguriert werden, wie die Nachnamen der Teilnehmer angezeigt werden: Die Namen können vollständig angezeigt werden oder der Nachname kann (wie bisher) abgekürzt auf die konfigurierte Länge (bisher war das immer '1') angezeigt werden.

*5.10.2020*
- Es kann eine Mindestteilnehmerzahl angegeben werden. Diese wird dann als kleiner roter "Badge" bei der Teilnehmerliste angezeigt, solange die Mindestanzahl nicht erreicht ist. Ansonsten hat das keine Auswirkungen, es werden natürlich nicht automatisch Termine gelöscht, wenn die Anzahl nicht erreicht wird. Es handelt sich lediglich um einen visuellen "Anreiz".
- Bug: Auf der Einladungsseite für Gäste fehlten die Angaben zum Startort.

*4.10.2020*
- Für Admins: Ein MeetUp kann als "wöchentlich" gekennzeichnet werden. Das MeetUp wird dann automatisch in der nächsten Woche wiederholt. Durch das "Veröffentlichungsdatum" kann gesteuert werden, wann das MeetUp für alle Nutzer sichtbar ist.
- Durch ein neues Feld kann ein Kartenlink (z.B. Google Maps) zum Startort der Ausfahrt angegeben werden, der dann als kleines Icon neben der Beschreibung des Startorts angezeigt wird.
- Kommntare können jetzt auch einen Link enthalten zusätzlich oder statt des Kommentartextes.
- Die Erstellung von Ausfahrten kann auf Admins eingeschränkt werden. Die regulären Nutzer können sich dann nur zu Ausfahrten an- und abmelden, aber keine eigenen Ausfahrten erstellen.
- Ein Admin kann den Zugang zum MeetUpPlanner für normale Nutzer sperren. Dies kann für notwendige Wartungsarbeiten genutzt werden oder auch, wenn die Corona-Regeln wieder verschärft werden und keine gemeinsamen Ausfahrten möglich sind.
- Admins können in der Kalendaransicht zwischen den Tagen navigieren, um auch nachträglich noch An- und Abmeldungen für Ausfahrten vornehmen zu können.
- Admins können sog. "InfoCards" anlegen. Diese werden ähnlich wie Ausfahrten in der Kalendaransicht gezeigt. Dies kann genutzt werden, um zusätzliche Infos wie Wetter oder ein "Whiteboard" anzuzeigen. Kommentare von den Nutzern können für diese InfoCards zugelassen werden. Ebenso kann pro InfoCard eingestellt werden wie lange die Kommentare gespeichert werden sollen.
