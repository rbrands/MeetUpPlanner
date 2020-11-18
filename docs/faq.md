[https://www.meetupplanner.de](https://www.meetupplanner.de)
# Häufige Fragen und Antworten

<dl>
  <dt>Wofür wurde der MeetUpPlanner entwickelt?</dt>
  <dd>
    Im März 2020 gab es den bisher ersten Lockdown in Deutschland allgemein und insbesondere auch in NRW. Sportliche Aktivitäten in der Gruppe kamen damit quasi komplett zum Erliegen. Als Rennradfahrer ist man zwar sowieso viel alleine unterwegs - aber immer? Als dann ab Mai 2020 schrittweise Lockerungen eingeführt wurden, taten sich alle schwer, wieder gemeinsame Ausfahrten zu organisieren. Grundprinzip dafür war und ist die Beschränkung der Gruppengröße und die Nachverfolgbarkeit der Kontakte. Zunächst haben wir angefangen mit Online-Listen durch Excel-Online usw. Aber das ist ein bisschen umständlich, funktioniert nicht vernünftig auf Smartphones und erfordert auch manuellen Aufwand. Genau in dieser Situation habe ich begonnen, diese WebApp zu entwickeln.
  </dd>
  
  <dt>Wer hat den MeetUpPlanner entwickelt?</dt>
  <dd>
    Ich habe diese WebApp als Privatperson und alleine entwickelt. Über mich: Schau dir einfach <a href="https://robert-brands.com">robert-brands.com</a> an. Ich verfolge mit der App keinerlei kommerziellen Interessen. Es gibt keine Werbung und die im Betrieb gesammelten Daten werden ausschließlich für den beschriebenen Zweck verwendet, d.h. für die Organisierung von Ausfahrten und zur Sicherstellung der Nachverfolgbarkeit im Sinne der Corona-Verordnung(en)
  </dd>
  
  <dt>Kostet der MeetUpPlanner etwas?</dt>
  <dd>
    Nein, ich stelle den MeetUpPlanner kostenlos zur Verfügung. Zum einen den Quellcode, der auf GitHub komplett und als Open Source verfügbar ist. Jeder kann ihn also verwenden und damit machen war er/sie will unter Beachtung der <a href="https://github.com/rbrands/MeetUpPlanner/blob/master/LICENSE">MIT Lizenz</a>... Außerdem betreibe ich natürlich eine Instanz und stelle sie zur Verfügung. Da gibt es natürlich irgendwann Kapazitätsgrenzen, ich kann da nicht beliebig viele Vereine aufnehmen ohne mehr Cloud-Ressourcen zu buchen, was dann wiederum mit Kosten verbunden wäre. 
  </dd>
  
  <dt>Gibt es so etwas wie den MeetUpPlanner auch vom BDR?</dt>
  <dd>
    Gute Frage ... Meines Wissens gibt es von den Radsportverbänden keinerlei Hilfen in irgendeiner Form für die Vereine, d.h. es bleibt den Vereinen komplett selbst überlassen, ob und wie sie sich organisieren. Die Verbände sind aus meiner Sicht in der Corona-Krise komplett abgetaucht. Dabei wäre es eine gute Idee, so etwas wie den MeetUpPlanner für alle Vereine anzubieten. Und auch ein "Buchungssystem" für RTFs usw. könnte gut gebraucht werden - auch nächstes Jahr noch ...
  </dd>
  
  <dt>Kann ich den MeetUpPlanner irgendwo ausprobieren?</dt>
  <dd>
    Ja, ich habe eine Demo-Version installiert, in der einfach Ausfahrten angelegt, gelöscht, verwaltet usw. werden können. Schick mir einfach eine Mail an <a href="mail@robert-brands.com">mail@robert-brands.com</a>.
  </dd>
  
  <dt>Wo wird der MeetUpPlanner bereits eingesetzt?</dt>
  <dd>
    Der MeetUpPlanner wird von zwei Radsportvereinen in Köln, um nicht zu sagen DEN beiden Radsportvereinen, eingesetzt: Ursprünglich wurde diese WebApp für die <a href="https://scuderia-suedstadt.de/">Scuderia Südstadt</a> entwickelt und ist seit Mitte August 2020 im Einsatz. Seit Anfang Oktober 2020 ist auch der <a href="https://dasimmerdabei.net/">RTC DSD</a> eingestiegen. Ebenfalls seit Oktober 2020 verwendet der Kölner <em>FB Giro</em> den MeetUpPlanner.
  </dd>
  
  <dt>Was muss ich tun, um den MeetUpPlanner auch für meinen Verein oder Radgruppe einzusetzen?</dt>
  <dd>
    Der MeetUpPlanner ist "mandantenfähig", d.h. eine Umgebung kann mehrere Vereine/Benutzergruppen "bedienen". Zwar wird die gleiche Datenbank usw. benutzt aber jede Gruppe sieht nur die eigenen Ausfahrten, auch die Einstellungen werden für jede Benutzergruppe getrennt. Die Trennung der Mandanten funktioniert über die URL, über die man den MeetUpPlanner aufruft. So führt z.B. club1.meetupplanner.de und club2.meetupplanner.de zu verschiedenen "Mandanten". Um einen weiteren Club einzurichten, muss ein eindeutiger Name wie gerade beschrieben eingerichtet werden. 
  Allerdings nähere ich mich mit der von mir betriebenen Installation langsam an die Kapaziätsgrenze. Was natürlich unabhängig davon immer geht: Der MeetUpPlanner ist komplett als Open Source verfügbar und kann von jedem mit etwas Kenntnissen im Betrieb von Diensten auf der Microsoft Azure Cloud eingerichtet werden.
  </dd>

  <dt>Ich möchte meine eigene Version des MeetUpPlanners installieren. Was muss ich tun?</dt>
  <dd>
    Alles was benötigt wird, ist in dem GitHub Repository vorhanden. Für eine eigene Version am besten das Repository "forken", Anpassungen zumindest an den Texten vornehmen, z.B. die "About"-Seite mit meiner Mail-Adresse usw. Um das ganze dann in Azure zu installieren, wird ein Azure App Service, eine Cosmos DB und eine Azure Functions App benötigt.
  
  Zur Installation siehe kurze Beschreibung in https://github.com/rbrands/MeetUpPlanner/wiki/Deployment
  </dd>
  
  <dt>Wieso ist die Administration in eine separate App gewandert?</dt>  
    <dd>
      Die "Anmeldung" beim MeetUpPlanner lediglich durch ein "Schlüsselwort" ist nur ein schwacher Schutz gegen unberechtigte Nutzung und ersetzt keine übliche Authentifizierung mit Benutzernamen, Kennwort und ggf. weitere Anmeldefaktoren. Für die Organisation von Ausfahrten und vor allem für die Anmeldung reicht das einfach Verfahren über die "Schlüsselwörter". Aber für die Administration und vor allem für den Zugriff auf Personendaten - nämlich beim Export von Kontaktlisten durch Administratoren - ist stärkere Schutz notwendig. Deswegen wurden diese Funktionen ausgelagert in die Admin-WebApp. 
    </dd>
  
  <dt>Gibt es den MeetUpPlanner auch in anderen Sprachen?</dt>
  <dd>
    Nein, zur Zeit ist der MeetUpPlanner nur in Deutsch.
  </dd>

</dl>
