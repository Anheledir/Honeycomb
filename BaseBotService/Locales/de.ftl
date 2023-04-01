#######################################################################################
# de.ftl
# Diese Datei enthält die Übersetzungsstrings für die Anwendung auf Deutsch.
# Weitere Informationen zu Fluent finden Sie unter https://projectfluent.org/fluent/guide/.

####################################
## Application Info
####################################

# Application Name
bot-name = Honeycomb

# Bot Website
bot-website = https://honeycombs.cloud/

# Bot description
bot-description = Honeycomb ist ein Discord-Bot, der Künstlern nützliche Funktionen bietet, um ihre Erfahrung auf Discord zu verbessern. Mit seinen Funktionen können Künstler ein Portfolio erstellen, zufällige Einträge daraus anzeigen, eine Preisliste für Aufträge verwalten und ihre Auftragswarteschlange im Auge behalten. Der Bot wird unter der MIT-Lizenz auf GitHub veröffentlicht.

# The version of the bot
# Variables:
#  - $version (String) - The version of the bot.
#  - $environment (String) - The environment the bot is running in.
bot-version = v{ $version } ({ $environment })

# The name of the bot with the version
bot-name-with-version = { bot-name } { bot-version }


####################################
## Core Module
####################################

follow-up-in-DM = Du hast eine DM erhalten!
not-available = n/v

# Interaction Buttons
button-close = Schließen
button-back = Zurück



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
uptime-format = { time-unit-days }, { time-unit-hours }, { time-unit-minutes } und { time-unit-seconds }



####################################
## Bot Module
####################################

uptime = Laufzeit
total-servers = Insgesamt Server
ping-response = :ping_pong: Es hat { $latency }ms gedauert, um Ihnen zu antworten!
documentation-filename = honeycomb_v{ $version }.json
documentation-created = Dies ist die aktuellste Dokumentation, extra für Sie erstellt!



####################################
## User Module
####################################

profile-select-setting = Bitte wählen Sie die Einstellung, die Sie ändern möchten.
profile-config = Wählen Sie die Benutzereinstellung, die Sie ändern möchten, oder klicken Sie auf Abbrechen, um zu beenden.
profile-saved = Ihr Profil wurde gespeichert.
profile-config-country = Wählen Sie das Land, in dem Sie leben.
profile-config-languages = Wählen Sie bis zu vier Sprachen, die Sie sprechen.
profile-config-gender = Wählen Sie Ihre bevorzugte Geschlechtsidentität.
profile-config-timezone = Bitte wählen Sie die Zeitzone, in der Sie leben.
profile-config-birthday-title = Geburtstag
profile-config-birthday-day = Tag
profile-config-birthday-day-placeholder = z. B. 29
profile-config-birthday-month = Monat
profile-config-birthday-month-placeholder = z. B. 03
profile-config-birthday-year = Jahr
profile-config-birthday-year-placeholder = { $exampleYear } oder leer lassen

profile-name = Name
profile-created = Erstellt am
profile-country = Wohnort
profile-languages = Sprachen
profile-gender = Geschlechtsidentität
profile-timezone = Zeitzone
profile-birthday = Geburtstag
profile-joined = Beigetreten am
profile-active = Zuletzt aktiv
profile-points = Serverpunkte
profile-roles = Rollen
profile-roles-none = Keine
profile-permissions = Berechtigungen
profile-permissions-none = Keine
profile-title = { $username } @ { $guildname }
profile-activity = Aktivitätsanzeige
profile-activity-rating = { $activityScore ->
    [0] Du bist ein Phantom!
    [1] So selten gesichtet wie Bigfoot!
    [2] Du bist wie ein Ninja, der sich im Schatten bewegt!
    [3] Der geheimnisvolle Umschweifer!
    [4] Ein Gelegenheits-Plauderer!
    [5] Du tauchst auf und ab wie ein Wiesel!
    [6] Ein echter Schmetterling unter den Plaudertaschen!
    [7] Stimmungsmacher der Party!
    [8] Du redest wie ein Wasserfall!
    [9] Ein wahrer Plaudermeister!
   [10] Du bist überall und nirgendwo!
   [11] Bist du etwa ein Bot?
   [12] Unaufhaltbar! Der Server platzt gleich!
   *[other] Ungültige Aktivitätspunktzahl
}



####################################
## Utilities
####################################

# Country names
country-united-states = Vereinigte Staaten von Amerika
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
gender-transgender-male = Transgender-Mann
gender-transgender-female = Transgender-Frau
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
language-thai = Thai
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
error-invalid-input = Ungültige Eingabe. Bitte versuchen Sie es erneut.

# Error message for connection issues
error-connection = Verbindung nicht möglich. Bitte überprüfen Sie Ihre Internetverbindung.

# Error message for rate limit
error-rate-limit = Sie haben das Nutzungslimit für diesen Befehl erreicht. Bitte warten Sie, bevor Sie es erneut versuchen.
