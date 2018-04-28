# AutoTagger
Find the best Instagram Hashtags for a specific Photo - Proof of Concept started developing at Cloud Solution Hackathon Hamburg 24.03. + 25.03.2018

## Architecture
![](https://github.com/Vittel/AutoTagger/raw/master/doc/architecture2.png)

## Crawler
see [doc/crawler.md](https://github.com/Vittel/AutoTagger/blob/master/doc/crawler.md)

## Database
see
  * [doc/better-database.md](https://github.com/Vittel/AutoTagger/blob/master/doc/better-database.md)
  * [doc/relational-query.md](https://github.com/Vittel/AutoTagger/blob/master/doc/relational-query.md)

Tested with CosmosDB von Azure (Graph), LiteDB (NoSQL) und MySQL DB (relational)

### Setup

Set following Environment Variables
- instatagger_mysql_ip
- instatagger_mysql_user
- instatagger_mysql_pw
- instatagger_mysql_db
- instatagger_clarifai_key

## Links
  * [Frontend](http://instatagger.do-epic-sh.it/)
  * [API](http://instataggerui.azurewebsites.net/swagger)
  * [Slack](https://dnughh.slack.com/messages/C9VD9KUTV/team/U7EU90J4S/)

## Contributors:
![Christian Seidlitz](https://avatars1.githubusercontent.com/u/1927076?s=50) [Christian Seidlitz](https://github.com/Vittel)<br />
![](http://via.placeholder.com/50x50) [Paul Stempel](https://github.com/tempel3)<br />
![](http://via.placeholder.com/50x50) [Florian Lierenfeld](https://github.com/soulseak)<br />
![Dario D. Müller](https://avatars1.githubusercontent.com/u/2358139?s=50) [Dario D. Müller](https://github.com/DarioDomiDE)
