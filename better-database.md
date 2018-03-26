# Database

## Example

| ID | ImgLink | ITags | Likes | Comments | Follower | MTags | popularity |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | xyz1 | I1, I2 | 200 | 40 | 2000 | M1, M2 | 0,12 |
| 2 | xyz2 | I3 | 120 | 5 | 10  | M3 | 12,5 |
| 3 | xyz3 | I4 | 300 | 400 | 1.000 | M4, M5, M6, M7, M8, M9, M10 | 0,7 |

  * popularity = (Likes+Comments)/Follower
  
## Berechnung
Beispiel-Bild hat MTags *M3*, *M4*, *M5*. Wir wollen die erfolgreichsten ITags bekommen.
  * Fragestellung: Welches sind die erfolgreichsten Bilder?
  * Suche in der Datenbank nach allen Einträgen, die MTag *M3* oder *M4* enthalten
  * für jeden Eintrag folgendes berechnen:
    * [overall] Die Anzahl der übereinstimmenden MTags (z.B. bei ID=3 -> *M4*)
    * [missing] Die Anzahl der fehlenden Hashtags, die auf Input- oder Entry-Seite zu viel sind. (z.B. ID=3 hat *M5* bis *M10* zu viel, sowie *M3* auf Input-Seite)
    * searchQuality =  [missing]/[overall]
    * relationQuality = searchQulity * popularity
  * Nun hat jeder Eintrag die tempotäte Spalte relationQuality. Sortiere alle Einträge nach relQuality absteigerd.
  * Bekomme die ersten 10 Einträge
Beispiel:

| relQuality | ITags |
| --- | --- |
| 7,0 | I5 |
| 3,0 | I6 |
| 1,0 | I6, I7, I8 |
| 0,5 | I8 |

  * Fragestellung: Welches sind die erfolgreichen ITags?
  * Einträge gruppieren nach mehrfach-Vorkommen und nach relQuality absteigend
    * Beispiel hier wäre: I6, I8, I5, I7
Diese Reihenfolge der ITags ist nun die finale Reihenfolge, wie erfolgreich die ITags sind
