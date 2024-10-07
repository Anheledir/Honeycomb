#######################################################################################
# de.ftl
# This file contains the localization strings for the application in German.
# See https://projectfluent.org/fluent/guide/ for more information about Fluent.
#######################################################################################

####################################
## Application Info
####################################

# Application Name
bot = Honeycomb
    .website = https://honeycombs.cloud/
    .github = https://github.com/Anheledir/Honeycomb
    .invite = https://discord.com/api/oauth2/authorize?client_id=1078320972286918686&permissions=2199023255383&scope=bot%20applications.commands
    .description = Honeycomb ist ein Discord-Bot, der entwickelt wurde, um Künstlern einige nützliche Funktionen zu bieten, die ihre Erfahrung auf Discord verbessern. Mit seinen Funktionen können Künstler ein Portfolio erstellen, zufällige Einträge daraus anzeigen, eine Preisliste für Aufträge verwalten und ihren Auftragsstatus verfolgen. Der Bot wird unter der MIT-Lizenz auf GitHub veröffentlicht.
    .version = v{ $version } ({ $environment })
    .name = { bot } { bot.version }

####################################
## Core Module
####################################

follow-up-in-DM = Du hast eine DM erhalten!
not-available = n/v

# Interaction Buttons
button-close = Schließen
button-back = Zurück
button-invite = Lade mich ein!
button-website = Webseite besuchen
button-github = GitHub besuchen

####################################
## Environment Service
####################################

# Time units with plural forms
# Variables:
#  - $days (Number) - The number of days.
#  - $hours (Number) - The number of hours.
#  - $minutes (Number) - The number of minutes.
#  - $seconds (Number) - The number of seconds.
time-unit-days = { $days ->
    [one] { $days } Tag
   *[other] { $days } Tage
}
time-unit-hours = { $hours ->
    [one] { $hours } Stunde
   *[other] { $hours } Stunden
}
time-unit-minutes = { $minutes ->
    [one] { $minutes } Minute
   *[other] { $minutes } Minuten
}
time-unit-seconds = { $seconds ->
    [one] { $seconds } Sekunde
   *[other] { $seconds } Sekunden
}

# Uptime format
uptime-format = { time-unit-days }, { time-unit-hours }, { time-unit-minutes }, und { time-unit-seconds }

####################################
## Bot Module
####################################

uptime = Betriebszeit
total-servers = Gesamtzahl der Server
ping-response = :ping_pong: Es hat { $latency }ms gedauert, um dir zu antworten!
documentation = JSON-Dokumentation für alle Bot-Befehle
    .filename = honeycomb_v{ $version }.json
    .created = Dies ist die neueste Dokumentation, frisch für dich erstellt!
invite = Lade mich auf deinen Server ein!
    .description = Klicke auf den untenstehenden Button, um mich auf deinen Server einzuladen. Ich helfe dir gerne!
    .button = { button-invite }
    .link = { bot.invite }

####################################
## Administrator Module
####################################

moderator-roles = Die Rolle '{ $role }' wurde von einem Administrator für Moderatoren festgelegt.

####################################
## User Module
####################################

profile-saved = Dein Profil wurde gespeichert.
profile-config = Bitte wähle die Einstellung aus, die du ändern möchtest.
    .country = Wähle das Land, in dem du lebst.
    .languages = Wähle bis zu vier Sprachen, die du sprichst.
    .gender = Wähle deine bevorzugte Geschlechtsidentität.
    .timezone = Bitte wähle die Zeitzone, in der du lebst.
profile-birthday = Geburtstag
    .day = Tag
    .day-placeholder = z.B. 29
    .month = Monat
    .month-placeholder = z.B. 03
    .year = Jahr
    .year-placeholder = { $exampleYear } oder leer lassen

profile = { $username } @ { $guildname }
    .name = Name
    .created = Erstellt am
    .country = Lebt in
    .languages = Spricht
    .gender = Geschlechtsidentität
    .timezone = Zeitzone
    .birthday = Geburtstag
    .joined = Beigetreten am
    .active = Zuletzt aktiv
    .points = Serverpunkte
    .roles = Rollen
    .roles-none = Keine
    .permissions = Berechtigungen
    .permissions-none = Keine
    .activity = Aktivitätsmesser
    .activity-rating = { $score ->
    [0] Du bist ein Geist!
    [1] Selten gesehen, wie Bigfoot!
    [2] Du bist wie ein Ninja, schleichst herum!
    [3] Der geheimnisvolle Wanderer!
    [4] Ein Gelegenheits-Plaudertasche!
    [5] Du tauchst auf und ab wie ein Erdmännchen!
    [6] Ein ziemlich geselliger Schmetterling!
    [7] Der Mittelpunkt der Party!
    [8] Du bist wie ein gesprächiger Papagei!
    [9] Eine wahre Plaudertasche!
   [10] Der allgegenwärtige Overlord!
   [11] Bist du ein Bot?
   [12] Unaufhaltsam! Uns geht der Serverspeicher aus!
   *[other] Ungültige Aktivitätsbewertung
}

####################################
## Utilities
####################################

# Country names
country-united-states = Vereinigte Staaten
country-canada = Kanada
country-united-kingdom = Vereinigtes Königreich
country-australia = Australien
country-germany = Deutschland
country-france = Frankreich
country-brazil = Brasilien
country-mexico = Mexiko
country-netherlands = Niederlande
country-sweden = Schweden
country-norway = Norwegen
country-denmark = Dänemark
country-finland = Finnland
country-spain = Spanien
country-italy = Italien
country-poland = Polen
country-japan = Japan
country-south-korea = Südkorea
country-china = China
country-turkey = Türkei
country-indonesia = Indonesien
country-philippines = Philippinen
country-austria = Österreich
country-swiss = Schweiz

# Gender Identities
gender-unknown = Unbekannt
gender-male = Männlich
gender-female = Weiblich
gender-non-binary = Nicht-binär
gender-transgender-male = Transgender Männlich
gender-transgender-female = Transgender Weiblich
gender-genderqueer = Genderqueer
gender-other = Andere

# Languages
language-unknown = Unbekannt
language-english = Englisch
language-german = Deutsch
language-french = Französisch
language-portuguese = Portugiesisch
language-spanish = Spanisch
language-dutch = Niederländisch
language-swedish = Schwedisch
language-norwegian = Norwegisch
language-danish = Dänisch
language-finnish = Finnisch
language-polish = Polnisch
language-russian = Russisch
language-japanese = Japanisch
language-korean = Koreanisch
language-chinese = Chinesisch
language-hindi = Hindi
language-turkish = Türkisch
language-indonesian = Indonesisch
language-filipino_tagalog = Filipino (Tagalog)
language-italian = Italienisch
language-arabic = Arabisch
language-thai = Thailändisch
language-vietnamese = Vietnamesisch
language-greek = Griechisch

# User configurations
userconfig-country = Land
userconfig-languages = Sprachen
userconfig-gender-identity = Geschlechtsidentität
userconfig-timezone = Zeitzone
userconfig-birthday = Geburtstag

####################################
## Error Messages
####################################

# Error message for invalid input
error-invalid-input = Ungültige Eingabe. Bitte versuche es erneut.

# Error message for connection issues
error-connection = Verbindung fehlgeschlagen. Bitte überprüfe deine Internetverbindung.

# Error message for rate limit
error-rate-limit = Du hast das Limit für diese Aktion erreicht. Bitte warte, bevor du es erneut versuchst.
