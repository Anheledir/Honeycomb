#######################################################################################
# fr.ftl
# This file contains the localization strings for the application in French.
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
    .description = Honeycomb est un bot Discord conçu pour fournir aux artistes des fonctions utiles pour améliorer leur expérience sur Discord. Avec ses fonctionnalités, les artistes peuvent créer un portfolio, afficher des entrées aléatoires, gérer une liste de prix pour les commandes et suivre leur file d'attente de commandes. Le bot est publié sous licence MIT sur GitHub.
    .version = v{ $version } ({ $environment })
    .name = { bot } { bot.version }

####################################
## Core Module
####################################

follow-up-in-DM = Vous avez reçu un DM !
not-available = n/a

# Interaction Buttons
button-close = Fermer
button-back = Retour
button-invite = Invite-moi !
button-website = Visiter le site web
button-github = Visiter GitHub

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
    [one] { $days } jour
   *[other] { $days } jours
}
time-unit-hours = { $hours ->
    [one] { $hours } heure
   *[other] { $hours } heures
}
time-unit-minutes = { $minutes ->
    [one] { $minutes } minute
   *[other] { $minutes } minutes
}
time-unit-seconds = { $seconds ->
    [one] { $seconds } seconde
   *[other] { $seconds } secondes
}

# Uptime format
uptime-format = { time-unit-days }, { time-unit-hours }, { time-unit-minutes }, et { time-unit-seconds }

####################################
## Bot Module
####################################

uptime = Temps de fonctionnement
total-servers = Nombre total de serveurs
ping-response = :ping_pong: Il m'a fallu { $latency }ms pour vous répondre !
documentation = Documentation JSON pour toutes les commandes du bot
    .filename = honeycomb_v{ $version }.json
    .created = Ceci est la documentation la plus récente, fraîchement créée pour vous !
invite = Invite-moi sur ton serveur !
    .description = Cliquez sur le bouton ci-dessous pour m'inviter sur votre serveur. Je serai heureux de vous aider !
    .button = { button-invite }
    .link = { bot.invite }

####################################
## User Module
####################################

profile-saved = Votre profil a été enregistré.
profile-config = Veuillez sélectionner le paramètre que vous souhaitez modifier.
    .country = Sélectionnez le pays dans lequel vous vivez.
    .languages = Sélectionnez jusqu'à quatre langues que vous parlez.
    .gender = Sélectionnez votre identité de genre préférée.
    .timezone = Veuillez sélectionner le fuseau horaire dans lequel vous vivez.
profile-birthday = Anniversaire
    .day = Jour
    .day-placeholder = ex. 29
    .month = Mois
    .month-placeholder = ex. 03
    .year = Année
    .year-placeholder = { $exampleYear } ou laissez vide

profile = { $username } @ { $guildname }
    .name = Nom
    .created = Créé le
    .country = Habite en
    .languages = Parle
    .gender = Identité de genre
    .timezone = Fuseau horaire
    .birthday = Anniversaire
    .joined = Rejoint le
    .active = Dernière activité
    .points = Points du serveur
    .roles = Rôles
    .roles-none = Aucun
    .permissions = Permissions
    .permissions-none = Aucune
    .activity = Indicateur d'activité
    .activity-rating = { $score ->
    [0] Vous êtes un fantôme !
    [1] Rarement vu, comme le Bigfoot !
    [2] Vous êtes comme un ninja, vous vous faufilez !
    [3] Le mystérieux vagabond !
    [4] Un bavard occasionnel !
    [5] Vous apparaissez et disparaissez comme un suricate !
    [6] Un joli papillon social !
    [7] La vie de la fête !
    [8] Vous êtes comme un perroquet bavard !
    [9] Un vrai moulin à paroles !
   [10] Le seigneur omniprésent !
   [11] Êtes-vous un bot ?
   [12] Inarrêtable ! Nous manquons d'espace serveur !
   *[other] Score d'activité invalide
}

####################################
## Utilities
####################################

# Country names
country-united-states = États-Unis
country-canada = Canada
country-united-kingdom = Royaume-Uni
country-australia = Australie
country-germany = Allemagne
country-france = France
country-brazil = Brésil
country-mexico = Mexique
country-netherlands = Pays-Bas
country-sweden = Suède
country-norway = Norvège
country-denmark = Danemark
country-finland = Finlande
country-spain = Espagne
country-italy = Italie
country-poland = Pologne
country-japan = Japon
country-south-korea = Corée du Sud
country-china = Chine
country-turkey = Turquie
country-indonesia = Indonésie
country-philippines = Philippines
country-austria = Autriche
country-swiss = Suisse

# Gender Identities
gender-unknown = Inconnu
gender-male = Masculin
gender-female = Féminin
gender-non-binary = Non-binaire
gender-transgender-male = Homme transgenre
gender-transgender-female = Femme transgenre
gender-genderqueer = Queer
gender-other = Autre

# Languages
language-unknown = Inconnu
language-english = Anglais
language-german = Allemand
language-french = Français
language-portuguese = Portugais
language-spanish = Espagnol
language-dutch = Néerlandais
language-swedish = Suédois
language-norwegian = Norvégien
language-danish = Danois
language-finnish = Finnois
language-polish = Polonais
language-russian = Russe
language-japanese = Japonais
language-korean = Coréen
language-chinese = Chinois
language-hindi = Hindi
language-turkish = Turc
language-indonesian = Indonésien
language-filipino_tagalog = Philippin (Tagalog)
language-italian = Italien
language-arabic = Arabe
language-thai = Thaïlandais
language-vietnamese = Vietnamien
language-greek = Grec

# User configurations
userconfig-country = Pays
userconfig-languages = Langues
userconfig-gender-identity = Identité de genre
userconfig-timezone = Fuseau horaire
userconfig-birthday = Anniversaire

####################################
## Error Messages
####################################

# Error message for invalid input
error-invalid-input = Entrée invalide. Veuillez réessayer.

# Error message for connection issues
error-connection = Impossible de se connecter. Veuillez vérifier votre connexion Internet.

# Error message for rate limit
error-rate-limit = Vous avez atteint la limite pour cette commande. Veuillez attendre avant de réessayer.
