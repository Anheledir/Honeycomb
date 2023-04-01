#######################################################################################
# es.ftl
# This file contains the localization strings for the application in Spanish.
# See https://projectfluent.org/fluent/guide/ for more information about Fluent.
#######################################################################################

####################################
## Application Info
####################################

# Application Name
bot-name = Honeycomb

# Bot Website
bot-website = https://honeycombs.cloud/

# Bot description
bot-description = Honeycomb is a Discord bot designed to provide artists with some useful functions to enhance their experience on Discord. With its features, artists can create a portfolio, display random entries from it, manage a commission price list, and keep track of their commission queue. The bot is released under the MIT license on GitHub.

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

follow-up-in-DM = You've got a DM!
not-available = n/a

# Interaction Buttons
button-close = Close
button-back = Go back



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
    [one] { $days } day
   *[other] { $days } days
}
time-unit-hours = { $hours ->
    [one] { $hours } hour
   *[other] { $hours } hours
}
time-unit-minutes = { $minutes ->
    [one] { $minutes } minute
   *[other] { $minutes } minutes
}
time-unit-seconds = { $seconds ->
    [one] { $seconds } second
   *[other] { $seconds } seconds
}

# Uptime format
uptime-format = { time-unit-days }, { time-unit-hours }, { time-unit-minutes }, and { time-unit-seconds }



####################################
## Bot Module
####################################

uptime = Uptime
total-servers = Total Servers
ping-response = :ping_pong: It took me { $latency }ms to respond to you!
documentation-filename = honeycomb_v{ $version }.json
documentation-created = This is the most recent documentation, freshly created just for you!



####################################
## User Module
####################################

profile-select-setting = Please select the setting you want to change.
profile-config = Select the user setting you want to change, or click cancel to exit.
profile-saved = Your profile has been saved.
profile-config-country = Select the country you are living in.
profile-config-languages = Select up to four languages you are speaking.
profile-config-gender = Select your preferred gender identity.
profile-config-timezone = Please select the timezone you are living in.
profile-config-birthday-title = Birthday
profile-config-birthday-day = Day
profile-config-birthday-day-placeholder = e.g. 29
profile-config-birthday-month = Month
profile-config-birthday-month-placeholder = e.g. 03
profile-config-birthday-year = Year
profile-config-birthday-year-placeholder = { $exampleYear } or leave empty

profile-name = Name
profile-created = Created at
profile-country = Living in
profile-languages = Speaking
profile-gender = Gender Identity
profile-timezone = Timezone
profile-birthday = Birthday
profile-joined = Joined at
profile-active = Last active
profile-points = Server points
profile-roles = Roles
profile-roles-none = None
profile-permissions = Permissions
profile-permissions-none = None
profile-title = { $username } @ { $guildname }



####################################
## Utilities
####################################

# Country names
country-united-states = United States
country-canada = Canada
country-united-kingdom = United Kingdom
country-australia = Australia
country-germany = Germany
country-france = France
country-brazil = Brazil
country-mexico = Mexico
country-netherlands = Netherlands
country-sweden = Sweden
country-norway = Norway
country-denmark = Denmark
country-finland = Finland
country-spain = Spain
country-italy = Italy
country-poland = Poland
country-japan = Japan
country-south-korea = South Korea
country-china = China
country-turkey = Turkey
country-indonesia = Indonesia
country-philippines = Philippines
country-austria = Austria
country-swiss = Swiss

# Gender Identities
gender-unknown = Unknown
gender-male = Male
gender-female = Female
gender-non-binary = Non-binary
gender-transgender-male = Transgender Male
gender-transgender-female = Transgender Female
gender-genderqueer = Genderqueer
gender-other = Other

# Languages
language-unknown = Unknown
language-english = English
language-german = German
language-french = French
language-portuguese = Portuguese
language-spanish = Spanish
language-dutch = Dutch
language-swedish = Swedish
language-norwegian = Norwegian
language-danish = Danish
language-finnish = Finnish
language-polish = Polish
language-russian = Russian
language-japanese = Japanese
language-korean = Korean
language-chinese = Chinese
language-hindi = Hindi
language-turkish = Turkish
language-indonesian = Indonesian
language-filipino_tagalog = Filipino (Tagalog)
language-italian = Italian
language-arabic = Arabic
language-thai = Thai
language-vietnamese = Vietnamese
language-greek = Greek

# User configurations
userconfig-country = Country
userconfig-languages = Languages
userconfig-gender-identity = Gender Identity
userconfig-timezone = Timezone
userconfig-birthday = Birthday



####################################
## Error Messages
####################################

# Error message for invalid input
error-invalid-input = Invalid input. Please try again.

# Error message for connection issues
error-connection = Unable to connect. Please check your Internet connection.

# Error message for rate limit
error-rate-limit = You have reached the rate limit for this command. Please wait before trying again.
