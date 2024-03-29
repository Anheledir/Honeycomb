﻿#######################################################################################
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
    .description = Honeycomb is a Discord bot designed to provide artists with some useful functions to enhance their experience on Discord. With its features, artists can create a portfolio, display random entries from it, manage a commission price list, and keep track of their commission queue. The bot is released under the MIT license on GitHub.
    .version = v{ $version } ({ $environment })
    .name = { bot } { bot.version }


####################################
## Core Module
####################################

follow-up-in-DM = You've got a DM!
not-available = n/a

# Interaction Buttons
button-close = Close
button-back = Go back
button-invite = Invite me!
button-website = Visit website
button-github = Visit GitHub



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
documentation = JSON-Documentation for all bot commands
    .filename = honeycomb_v{ $version }.json
    .created = This is the most recent documentation, freshly created just for you!
invite = Invite me to your server!
    .description = Click the button below to invite me to your server. I'll be happy to help you!
    .button = { button-invite }
    .link = { bot.invite }



####################################
## User Module
####################################

profile-saved = Your profile has been saved.
profile-config = Please select the setting you want to change.
    .country = Select the country you are living in.
    .languages = Select up to four languages you are speaking.
    .gender = Select your preferred gender identity.
    .timezone = Please select the timezone you are living in.
profile-birthday = Birthday
    .day = Day
    .day-placeholder = e.g. 29
    .month = Month
    .month-placeholder = e.g. 03
    .year = Year
    .year-placeholder = { $exampleYear } or leave empty

profile = { $username } @ { $guildname }
    .name = Name
    .created = Created at
    .country = Living in
    .languages = Speaking
    .gender = Gender Identity
    .timezone = Timezone
    .birthday = Birthday
    .joined = Joined at
    .active = Last active
    .points = Server points
    .roles = Roles
    .roles-none = None
    .permissions = Permissions
    .permissions-none = None
    .activity = Activity Meter
    .activity-rating = { $score ->
    [0] You're a ghost!
    [1] Rarely seen, like Bigfoot!
    [2] You're like a ninja, sneaking around!
    [3] The mysterious wanderer!
    [4] A casual chit-chatter!
    [5] You pop in and out like a meerkat!
    [6] A pretty social butterfly!
    [7] The life of the party!
    [8] You're like a talkative parrot!
    [9] A true chatterbox!
   [10] The ever-present overlord!
   [11] Are you a bot?
   [12] Unstoppable! We're running out of server space!
   *[other] Invalid activity score
}




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
