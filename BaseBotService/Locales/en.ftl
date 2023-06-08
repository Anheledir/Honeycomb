#######################################################################################
# en.ftl
# This file contains the localization strings for the application in English.
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
button-abort = Cancel
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
## Admin Module
####################################

guild-config-saved = Your guild settings have been saved.
guild-config = Please select the setting you want to change.
    .modrole = Select up to five (5) roles that are allowed to use the moderation commands.
    .artistroles = Select up to five (5) roles that are allowed to use the artist commands.



####################################
## Polls Module
####################################

polls = Server-Polls
    .create = Create a new poll
    .create-msg-processing = Creating a new poll ...
    .create-poll-title = The title of your poll
    .create-poll-description = The description of your poll
    .create-poll-roles = Select the roles that are allowed to vote
    .create-poll-public-results = Show the results to everyone
    .create-poll-voters-hidden = Hide the voters
    .create-poll-option-adding = Add a new option
    .create-poll-option-text = The text of the option
    .create-poll-option-emoji = The emoji of the option
    .create-poll-end = { $duration ->
    [0] 1 hour
    [1] 2 hours
    [2] 4 hours
    [3] 8 hours
    [4] 12 hours
    [5] 24 hours
    [6] until midnight
    [7] 2 days (48 hours)
    [8] 3 days (72 hours)
    [9] 1 week (7 days)
   *[other] manual (until it is closed)
}



####################################
## User Module
####################################

profile-saved = Your profile has been saved.
profile-config = Please select the setting you want to change.
    .country = Select the country you are living in.
    .languages = Select up to four languages you are speaking.
    .gender = Select your preferred gender identity.
    .timezone = Please select the timezone you are living in.
    .button = Edit My Profile
    .emoji = :pencil:
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
    .activity-calc = Still taking notes, come back tomorrow ...
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
## Achievements
####################################

achievements = Achievement: { $name }
    .title = Achievements
    .description = You can earn achievements by participating in servers. The more you participate, the more achievements you can earn. You can see your current achievements by using the command `/user achievements`.
    .no-achievements = You don't have any achievements yet. You can earn achievements by participating in servers. The more you participate, the more achievements you can earn. You can see your current achievements by using the command `/user achievements`.
    .none-other = { $username } doesn't have any achievements yet.
    .none-self = You don't have any achievements yet.
    .issued = { $username } earned the achievement "{ $name }"! { $points ->
        [one] ({ $points } point)
       *[other] ({ $points } points)
    }

achievement-event-easter = Easter { $year }
    .description = You've participated in the Easter event { $year }.
    .emoji = :egg:
    .image = https://i.imgur.com/removed.png
    .notification = Happy Easter, { $username }! :rabbit: :hatching_chick: :hibiscus:
    .activity = Happy Easter! :rabbit: :hatching_chick: :hibiscus:



####################################
## Activities
####################################
default-activity = { bot.website }



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

# Guild configurations
guildconfig-modroles = Moderator Roles
guildconfig-artistroles = Artist Roles



####################################
## Error Messages
####################################

# Error message for invalid input
error-invalid-input = Invalid input. Please try again.

# Error message for connection issues
error-connection = Unable to connect. Please check your Internet connection.

# Error message for rate limit
error-rate-limit = You have reached the rate limit for this command. Please wait before trying again.

# Error message for missing guild
error-guild-missing = This command can only be used in a server.

# Error message when the guild could not be loaded or created in the database
error-guild-load = Unable to load the server. Please try again later.

# Error message when the user tried to create a new poll in a non-text channel
error-poll-create-invalid-channel = You can only create polls in text channels.

# Error message when the user tried to create a new poll in a channel where he doesn't doesn't have the required permissions
error-poll-create-no-permissions = You don't have the required permissions to create polls in this channel.

# Error message when the poll-id couldn't be found in the database
error-poll-create-invalid-poll = Unable to find the poll. Please try again.

# Error message when the message that corresponds to the poll-id couldn't be found
error-poll-invalid-message = Unable to find the discord message the poll should be shown in. Was it accidentally deleted by someone?
