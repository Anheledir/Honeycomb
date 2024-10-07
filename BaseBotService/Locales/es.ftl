#######################################################################################
# es.ftl
# This file contains the localization strings for the application in Spanish.
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
    .description = Honeycomb es un bot de Discord diseñado para proporcionar a los artistas algunas funciones útiles para mejorar su experiencia en Discord. Con sus características, los artistas pueden crear un portafolio, mostrar entradas aleatorias de él, gestionar una lista de precios para encargos y llevar un seguimiento de su cola de encargos. El bot se publica bajo la licencia MIT en GitHub.
    .version = v{ $version } ({ $environment })
    .name = { bot } { bot.version }

####################################
## Core Module
####################################

follow-up-in-DM = ¡Tienes un mensaje directo!
not-available = n/d

# Interaction Buttons
button-close = Cerrar
button-back = Regresar
button-invite = ¡Invítame!
button-website = Visitar sitio web
button-github = Visitar GitHub

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
    [one] { $days } día
   *[other] { $days } días
}
time-unit-hours = { $hours ->
    [one] { $hours } hora
   *[other] { $hours } horas
}
time-unit-minutes = { $minutes ->
    [one] { $minutes } minuto
   *[other] { $minutes } minutos
}
time-unit-seconds = { $seconds ->
    [one] { $seconds } segundo
   *[other] { $seconds } segundos
}

# Uptime format
uptime-format = { time-unit-days }, { time-unit-hours }, { time-unit-minutes }, y { time-unit-seconds }

####################################
## Bot Module
####################################

uptime = Tiempo de actividad
total-servers = Servidores totales
ping-response = :ping_pong: ¡Me tomó { $latency }ms responderte!
documentation = Documentación JSON de todos los comandos del bot
    .filename = honeycomb_v{ $version }.json
    .created = ¡Esta es la documentación más reciente, recién creada solo para ti!
invite = ¡Invítame a tu servidor!
    .description = Haz clic en el botón de abajo para invitarme a tu servidor. ¡Estaré encantado de ayudarte!
    .button = { button-invite }
    .link = { bot.invite }

####################################
## User Module
####################################

profile-saved = Tu perfil ha sido guardado.
profile-config = Por favor, selecciona la configuración que deseas cambiar.
    .country = Selecciona el país en el que vives.
    .languages = Selecciona hasta cuatro idiomas que hablas.
    .gender = Selecciona tu identidad de género preferida.
    .timezone = Por favor, selecciona la zona horaria en la que vives.
profile-birthday = Cumpleaños
    .day = Día
    .day-placeholder = ej. 29
    .month = Mes
    .month-placeholder = ej. 03
    .year = Año
    .year-placeholder = { $exampleYear } o deja en blanco

profile = { $username } @ { $guildname }
    .name = Nombre
    .created = Creado el
    .country = Vive en
    .languages = Habla
    .gender = Identidad de género
    .timezone = Zona horaria
    .birthday = Cumpleaños
    .joined = Se unió el
    .active = Última actividad
    .points = Puntos del servidor
    .roles = Roles
    .roles-none = Ninguno
    .permissions = Permisos
    .permissions-none = Ninguno
    .activity = Medidor de actividad
    .activity-rating = { $score ->
    [0] ¡Eres un fantasma!
    [1] Raramente visto, ¡como Bigfoot!
    [2] ¡Eres como un ninja, te escabulles!
    [3] ¡El vagabundo misterioso!
    [4] ¡Un conversador casual!
    [5] ¡Apareces y desapareces como un suricata!
    [6] ¡Una bonita mariposa social!
    [7] ¡El alma de la fiesta!
    [8] ¡Eres como un loro hablador!
    [9] ¡Un verdadero parlanchín!
   [10] ¡El señor omnipresente!
   [11] ¿Eres un bot?
   [12] ¡Imparable! ¡Nos estamos quedando sin espacio en el servidor!
   *[other] Puntuación de actividad inválida
}

####################################
## Utilities
####################################

# Country names
country-united-states = Estados Unidos
country-canada = Canadá
country-united-kingdom = Reino Unido
country-australia = Australia
country-germany = Alemania
country-france = Francia
country-brazil = Brasil
country-mexico = México
country-netherlands = Países Bajos
country-sweden = Suecia
country-norway = Noruega
country-denmark = Dinamarca
country-finland = Finlandia
country-spain = España
country-italy = Italia
country-poland = Polonia
country-japan = Japón
country-south-korea = Corea del Sur
country-china = China
country-turkey = Turquía
country-indonesia = Indonesia
country-philippines = Filipinas
country-austria = Austria
country-swiss = Suiza

# Gender Identities
gender-unknown = Desconocido
gender-male = Masculino
gender-female = Femenino
gender-non-binary = No binario
gender-transgender-male = Hombre transgénero
gender-transgender-female = Mujer transgénero
gender-genderqueer = Queer
gender-other = Otro

# Languages
language-unknown = Desconocido
language-english = Inglés
language-german = Alemán
language-french = Francés
language-portuguese = Portugués
language-spanish = Español
language-dutch = Neerlandés
language-swedish = Sueco
language-norwegian = Noruego
language-danish = Danés
language-finnish = Finés
language-polish = Polaco
language-russian = Ruso
language-japanese = Japonés
language-korean = Coreano
language-chinese = Chino
language-hindi = Hindi
language-turkish = Turco
language-indonesian = Indonesio
language-filipino_tagalog = Filipino (Tagalog)
language-italian = Italiano
language-arabic = Árabe
language-thai = Tailandés
language-vietnamese = Vietnamita
language-greek = Griego

# User configurations
userconfig-country = País
userconfig-languages = Idiomas
userconfig-gender-identity = Identidad de género
userconfig-timezone = Zona horaria
userconfig-birthday = Cumpleaños

####################################
## Error Messages
####################################

# Error message for invalid input
error-invalid-input = Entrada inválida. Por favor, inténtalo de nuevo.

# Error message for connection issues
error-connection = No se puede conectar. Por favor, verifica tu conexión a Internet.

# Error message for rate limit
error-rate-limit = Has alcanzado el límite de este comando. Por favor, espera antes de intentarlo de nuevo.
