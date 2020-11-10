[https://www.meetupplanner.de](https://www.meetupplanner.de)
# Benutzerhinweise

Es ist sicherlich kein Benutzerhandbuch notwendig, der MeetUpPlanner sollte selbsterklärend sein: Ausfahrt anlegen, ändern, löschen, teilnehmen, kommentieren. Hoffentlich alles naheliegend wie es funktioniert.

Hier sollen die etwas versteckteren Features beschrieben werden.

## Features für alle Benutzer
<dl>
  <dt id="pwa">Installation als App</dt>
  <dd>
  <p>
    Den MeetUpPlanner kann man auch als App und zwar als PWA (<a href="https://de.wikipedia.org/wiki/Progressive_Web_App" target="_blank">"Progressive Web App"</a>)       installieren. Damit verkürzt sich die Startzeit und die App bleibt länger aktiv, um Updates zu den Ausfahrten direkt anzeigen zu können.
    </p>
<p>  
Die Installation ist ganz einfach: Auf Android, auf dem PC im Edge und Chrome-Browser wie üblich den MeetUpPlanner aufrufen. Oben rechts in der Addresszeile ist ein kleines +, über das man die Installation startet. Unter iOS im Safari-Browser geht man auf "Teilen" und dann "Zum Home-Bildschirm". Der MeetUpPlanner wird dann mit Logo auf dem Home-Bildschirm oder im Start-Menü (PC) angezeigt und kann von da auch einfach wieder deinstalliert werden.
</p>
<p>
Außerdem ist es möglich, sich auf Android, PC (kurzum Chrome-basierte Browser, leider noch kein iOS/Safari) über Änderungen an Ausfahrten (Absage, Terminänderungen usw.) und über neue Kommentare benachrichtigen zu lassen. Beim Aufruf der Kalenderseite gibt es eine entsprechende Popup-Abfrage oder je nach Einsellung in der Adresszeile ein kleines "Glöckchen", über das man die Benachrichtungen zulassen oder eben auch ablehnen kann.
 Die Benachrichtigungen lassen sich nachträglich auch wieder in den Einstellungen des Browsers abstellen.
</p>
</dd>

<dt>"Private" Ausfahrten</dt>
  <dd>
    <p>
      Normalerweise sind alle Termine, die angelegt werden, für alle sichtbar, die sich mit dem normalen Benutzer-Schlüsselwort oder dem Admin-Schlüsselwort anmelden. Wird aber ein Termin angelegt oder ein existierender geändert, gibt es weiter unten über das Eingabefeld "Private Ausfahrt?" die Möglichkeit, für den Termin ein Schlüsselwort zu vereinbaren. Ein so gekennzeichneter Termin wird anderen Benutzern nur dann angezeigt, wenn sie eben dieses Schlüsselwort in einem der drei Felder "Optionale Schlüsselwörter für private Ausfahrten" auf der Startseite eingeben.
  </p>
  <p>
    Beispiel: Wenn eine vereinsinterne Ausfahrt organisiert werden soll, könnte diese mit dem Schlüsselwort "MeinClub" versehen werden. Dieses Schlüsselwort wird den Vereinsmitgliedern per Mail/SMS mit geteilt. Die geben "MeinClub" im Feld "Optionale Schlüsselwörter" ein und sehen so - und nur sie - die besagte Ausfahrt. Dies kann also genutzt werden, um den Kreis der potentiellen Teilnehmer einzuschränken und trotzdem die Nachverfolgbarkeit zu gewährleisten.
  </p>
  </dd>
  <dt>Gäste einladen</dt>
  <dd>
    <p>
      Um den MeetUpPlanner zu nutzen, wird normalerweise ein "Schlüsselwort" benötigt, nicht nur um ggf. Ausfahrten anzulegen, sondern auch für die Anmeldung. Um jemanden spontan oder auch gezielt für nur eine Ausfahrt mit zu nehmen, gibt es die "Gastfunktion". Über das Icon <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/external-link.svg" height="14" /> gelangt man auf eine Einladungsseite. Diese enthält neben einen Link, der per Mail/SMS oder Messenger verschickt werden kann auch einen QR-Code, der ebenfalls zur Gastseite führt. Auf dieser kann sich der Gast nur mit Vor-/Nachnamen und Kontaktinfo (Telefon oder E-Mail) für diese eine Ausfahrt an- und ggf. auch abmelden. 
  </p>
  <p>
    Sollen Gäste zu einer Gruppe von Ausfahrten (Beispiel "WTT") eingeladen werden, gibt es es eine weitere Möglichkeit: Für eine Ausfahrt kann ein "Scope" für Gäste angegeben werden. Die so gekennzeichneten Ausfahrten sind dann unter dem Link /guests/scope erreichbar. Die Gäste können sich dort anmelden und auch wieder abmelden, sie sehen allerdings keine Namen und auch keine Kommentare.
  </p>
  </dd>  
  
  <dt>Termin kopieren</dt>
  <dd>
    Hat man einen Termin angelegt, wird auch der kleine Button <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/fork.svg" height="14"/> angezeigt, über den ein Termin in die nächste Woche kopiert werden kann. Die Idee ist hier, dass man Termine, die immer wieder angeboten werden, nicht komplett neu anlegen muss.
  </dd>
  
  <dt>An- und Abmeldung für andere</dt>
  <dd>
    Als Guide oder Admin kann man die An- und Abmeldung für eine Ausfahrt auch für andere Teilnehmer übernehmen, falls die gerade z.B. nicht ihr Smartphone zur Hand haben. Über das Icon <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/transfer.svg" height="14"/> gelangt man auf eine Anmeldeseite, in der man die Kontaktdaten zum Teinehmer eingeben kann.
  </dd>
  
  <dt>Neuer Guide</dt>
  <dd>
    Hat man eine Ausfahrt erstellt und ist damit glücklicher Guide, kann man sich nicht einfach abmelden. Wenn jetzt aber etwas dazwischen kommt und man nicht die ganze Ausfahrt löschen möchte, kann man sie über das Icon <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/share-boxed.svg" height="14"/> an einen Teilnehmer übertragen. Wahrscheinlich ist es eine gute Idee, vorher zu fragen, ob das ok ist...
  </dd>
  
  <dt>Ausfahrt absagen</dt>
  <dd>
    <p>
      Manchmal muss man eine Ausfahrt auch absagen, wenn sich z.B. nicht genug Mitfahrer finden oder das Wetter einfach zu schlecht ist. Als Guide einer Ausfahrt oder als Admin kann man eine Ausfahrt zwar über den Button <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/trash.svg" height="14"/> löschen. Nach einer Rückfrage, ob man wirklich löschen will, wird die Ausfahrt komplett inkl. Anmeldungen und Kommentaren gelöscht. Es gibt dann keine Möglichkeit, die anderen entsprechend zu informieren. Der bessere Weg ist hier, eine Ausfahrt zunächst über <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/x.svg" height="14"/> als abgesagt zu markieren und vielleicht noch einen Kommentar einzufügen oder den Beschreibungstext zu ändern. Eine Anmeldung zur Ausfahrt ist dann nicht mehr möglich (Abmeldung schon). Jeder kann und sollte so vor der Ausfahrt noch einmal nach schauen, ob die Ausfahrt auch stattfindet. Eine Ausfahrt, die als abgesagt markiert wurde, wird auch in der Liste zur Kontaktverfolgung nicht berücksichtigt. 
  </p>
  <p>
    Die Absage kann auch leicht wieder rückgängig gemacht werden - einfach noch einmal den Button <img src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/x.svg" height="14"/> drücken ...
  </p>
  </dd>
  <dt>Mindestteilnehmerzahl</dt>
  <dd>
    Man kann für eine Ausfahrt eine Mindestteilnehmeranzahl angeben. Diese wird dann als kleiner "Badge" bei der Teilnehmeranzahl angezeigt. Ansonsten hat das keine Auswirkungen, es wird also keine Ausfahrt automatisch abgesagt oder sogar gelöscht, wenn die Teilnehmeranzahl nicht erreicht wird. Es wird nur ein kleiner visueller "Anreiz" gesetzt.
  </dd>
</dl>

## Weitere Features für Admins
Mit dem sog. "Admin-Schlüsselwort" stehen einem weitere Funktionen zur Verfügung:  
<dl>
  <dt>Zusätzliche Teilnehmer</dt>
  <dd>
    Admins können auf zwei Arten zusätzliche Teilnehmer anmelden bzw. einladen: Zum einen können Admins eine Ausfahrt "überbuchen", d.h. sie können andere Teilnehmer hinzufügen auch wenn die maximale Teilnehmeranzahl überschritten ist. Dahinter steckt die Idee, dass es im Sinne der Nachverfolgbarkeit besser ist, jemanden zu registrieren auch wenn die Ausfahrt eigentlich voll ist, als sie/ihn "stillschweigend" mitfahren zu lassen.
  </dd>
  
  <dt>Regelmäßige Termine</dt>
  <dd>
    Durch die Kombination von mehreren Features wird die Erstellung von regelmäßigen, wöchentlichen Terminen unterstützt.
    <ul>
      <li>
        Es gibt eine entsprechende Checkbox für Termine, um sie als wöchentlich wiederkehrend zu kennzeichnen. Für jeden Tag werden dann zentral alle Termine geprüft und die als wiederkehrend gekennzeichneten werden in die Folgewoche kopiert, natürlich ohne Anmeldungen und ohne Kommentare. Außerdem können Admins für Ausfahrten die max. Teilnehmeranzahl bis zu einem separat konfigurierten Max.-Wert hochsetzen. Dies ist gedacht für ggf. besondere Vereinsausfahrten oder ähnliches.
      </li>
      <li>
        Es kann ein "Veröffentlichungsdatum" für Termine angegeben werden. Damit lässt sich steuern, wann die Termine für alle sichtbar sind. Beispiel: Man richtet eine wöchentliche Dienstagsausfahrt ein und trägt als Veröffentlichungsdatum den Mittwoch ein. Dann sehen alle immer nur die aktuelle Ausfahrt und werden nicht von zu vielen Terminen irritiert ... 
      </li>
      <li>
        Normalerweise haben Termine immer einen "Guide". Als Admin kann man aber auch einen Termin "ohne Guide" einrichten. Auch das ist gedacht vor allem für wöchentliche Termine, die unabhängig von einer Person stattfinden. 
      </li>
    </ul>
  </dd>
   
  <dt>Wechsel von Ausfahrten "mit Guide" zu Ausfahrten "ohne Guide"</dt>
  <dd>
  <p>
    Ausfahrten können von Admins auch "ohne Guide" angelegt werden, z.B. für regelmäßige Termine, für die kein Guide benötigt wird. Falls eine Ausfahrt geändert wird von "ohne Guide" zu "mit Guide" wird der aktuelle Nutzer (also ein Admin) als neuer Guide eingetragen. In einem weiteren Schritt kann die Ausfahrt ggf. dann einem anderen Teilnehmer zugewiesen werden.
  </p>
  <p>
    Wird eine Ausfahrt von "mit Guide" zu "ohne Guide" geändert, wird der aktuelle Guide als "einfacher" Teilnehmer hinzugefügt.
  </p>
  </dd>
  
  <dt>Infoboxen</dt>
  <dd>
    Admins können sog. "Infoboxen" anlegen, die ähnlich wie Termine angezeigt werden können. So können zusätzliche Infos zur Bedienung, Wetteraussichten usw. angezeigt werden. Es lässt sich einstellen, ob Kommentare zugelassen sind und wie lange diese gespeichert werden sollen. Auf diese Weite kann ein "Schwarzes Brett" umgesetzt werden, über das die Nutzer sich austauschen können.
  </dd>
</dl>

## Admin-Tool
Die eigentliche Administration des MeetUpPlanners erfolgt über das Administrationstool https://admin.meetupplanner.de. Die Administrations-Funktionen wurden aus Sicherheitsgründen ausgelagert. Das Admin-Tool kann nur nach Authentifizierung und der Zuweisung entsprechender Rechte genutzt werden. Zur Authentifizierung werden die Dienste von Microsoft, Google, Facebook, Twitter oder GitHub unterstützt. Das heißt, man meldet sich mit seinem Account bei einem dieser Dienste an. Um Administrator zu werden, muss man zunächst mitteilen, mit welchem Account von den eben genannten Diensten man zugreifen möchte, damit ein entsprechender Einladungslink erstellt werden kann.

Im Admin-Tool stehen die folgenden Funktionen (u.a.) zur Verfügung:
<dl>
  <dt>Begrüßungstexte konfigurieren</dt>
  <dd>
    Um Hinweise an die Nutzer zu geben, können in der Adminstration verschiedene Texte eingegeben und einfach geändert werden. Formatierung der Texte und das Einfügen von Links ist auch jeweils möglich. Folgende Texte können edititiert werden:
    <ul>
      <li>Welcome Nachricht: Text auf der Startseite inkl. optionalen Logo. Hier können direkt Hinweise darauf gegeben werden, wie der MeetUpPlanner genutzt werden sollte.</li>
      <li>
        Whiteboard Nachricht: Text der über der zentralen Seite mit den Ausfahrten steht. Auch hier können gut Hinweise auf die Regelungen um die Ausfahrten gegeben werden. Aber am besten kurz fassen, ansonsten müssen alle auf dem Handy immer erst nach unten scrollen, um die Ausfahrten zu sehen. Das kommt nicht so gut an ...
      </li>
      <li>
        Nachricht für neue MeetUps: Dieser Text wird auf der Seite zur Anlage einer neuen Ausfahrt angezeigt. Also hier am besten ein kurzer Hinweis z.B. zur Gruppengröße oder andere Spielregeln.
      </li>
    </ul>
  </dd>

  <dt>Einstellung "Sollen nur Admins MeetUps anlegen dürfen?"</dt>
  <dd>
    Diese Einstellung in der Administration regelt, wer MeetUps anlegen darf. Wird dies auf Admins eingeschränkt, können Nutzer, die nur das "normale" Schlüsselwort eingeben,
    nur alle MeetUps sehen und sich an- und abmelden und auch kommentieren. Die Admins erstellen die MeetUps. Dies ist für Clubs gedacht, die nur einige Ausfahrten kontrolliert zu festen Termine anbieten wollen.
  </dd>

<dt>"Badges" für Wochentage</dt>
  <dd>
    In der Administration können für die Wochentage sog. "Badges" konfiguriert werden, um Ausfahrten an diesen Tagen besonders zu kennzeichnen. Beispiel: "ScuDi" für den Scuderia Dienstag und "ScuSo" für den Sonntag. Außerdem ist es möglich, die Anlage von neuen Ausfahrten auf diese Tage zu beschränken.
  </dd>

<dt>Zugang sperren</dt>
  <dd>
    Der Zugang zum MeetUpPlanner kann gesperrt werden und eine Meldung dazu kann auch konfiguriert werden. Dies kann z.B. nötig sein, wenn die Corona-Schutzverordnung keine gemeinsamen Ausfahrten zulassen sollte.
  </dd>
</dl>

