# Quality Improvement: "meat" vs "Vegan"

Problem: "#meat" vs "#vegaaaan". Ein foto auf dem Essen zu sehen ist und der ImageProcessor "meat" erkannt hat, bekommt "meat" und "Vegan" Hashtags zugewiesen.

Frage: Wie kann man am besten herausfinden, dass diese zwei hashtags NIE miteinander ins selbe Bild (welchea mtag="meat" hat) passen. Also darf die Software diese auch nicht ins selbe Bild packen.

## Ausschluss Regeln (Subtags-Suche) für Konflikte wie "Meat" vs. "Vegan"

[hashtagsVorauswahl] = "meat", "vegas", ...
[ausschlussDict<search, conflicted>]

foreach [hashtagsVorauswahl] as [searchHashtag]
{

	- gib alle fotos [photos], die tag [searchHashtag] haben
	- [hArr] gib alle hashtags von [photos] (entweder nicht-unique oder mit amount-counter)
	foreach [hashtagsVorauswahl] as [hashtag]
	{
		- zähle die vorkommen von [hashtag] innerhalb von [hArr]
		- wenn das Vorkommen 0 oder "sehr-gering" ist
			- [ausschlussDict].Add([searchHashtag], [hashtag]) 
	}
}
foreach [ausschlussDict] as [entry]
	[hashtagsVorauswahl].Remove([entry].value)