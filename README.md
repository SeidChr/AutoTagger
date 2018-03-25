# AutoTagger
Find Instagram Tags by uploading a Photo

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

