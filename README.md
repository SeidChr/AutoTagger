# AutoTagger
Find Instagram Tags by uploading a Photo - Cloud Solution Hackathon 24. + 25.03.2018

## Architecture
![](https://github.com/Vittel/AutoTagger/raw/master/doc/architecture.jpg)

## TaggingProvider
derzeit Konsolenanwendung
- ToDo REST Api 

## UserInterface
Frontend:
```/index```

Rest API
- POST /image -> body: form-data -> param "link"
- POST /image/upload -> param "file" with image

## Hosting
Frontend with Continues Delivery
http://autotaggerui.azurewebsites.net/index

### ToDos
  * [ ] Datenbank: Qualität der Tags erhöhen, evlt durch Nutzung einer anderen DB
  * [ ] Schauen, ob es Instagram Datensätzze schon iwo gibt, damit nicht selbst crawlen
  * [ ] Crawling als Job und in Azure hosten
  * [ ] Service-based architecture
  * [ ] Tests: Die Aktuellen überarbeiten, Dependencies mocken als Unit u. Acceptance-Tests verpacken
  * [ ] DI ordentlich
  * [ ] UserInterface Service: Views entfernen und wieder als Rest API
  * [ ] SPA Frontend bauen mit Angular o.ö. und welches Rest API nutzt

# Links
  * https://dnughh.slack.com/messages/C9VD9KUTV/team/U7EU90J4S/

## Contributors:
![Christian Seidlitz](https://avatars1.githubusercontent.com/u/1927076?s=50) [Christian Seidlitz](https://github.com/Vittel)<br />
[Florian Lierenfeld](https://github.com/soulseak)<br />
[Paul Stempel](https://github.com/tempel3)<br />
![Dario D. Müller](https://avatars1.githubusercontent.com/u/2358139?s=50) [Dario D. Müller](https://github.com/DarioDomiDE)
