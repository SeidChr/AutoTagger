# A query for a relational DB based on better-batabase.md

```
SELECT
#sub2.id as photoId,
#i.id as itagId,
i.name
#relationQuality,
#matches,
#sum(matches) as sum
#count(i.name) as imagesCount
FROM itags as i
LEFT JOIN photo_itag_rel as rel ON rel.itagId = i.id
LEFT JOIN
(
    #SELECT p.id,
    #matches,
    #count(m.name) as overall,
    #count(m.name)-matches as missing,
    #[matches]/[overall]
    #matches / count(m.name) as searchQuality,
    #popularity,
    #[searchQuality] + [popularity]
    #(count(m.name)-matches) / count(m.name) + popularity as relationQuality) / (count(m.name))) * popularity as relationQuality
    #FROM photos as p
    #LEFT JOIN mtags as m ON m.photoId = p.id
    #LEFT JOIN
    #(
        SELECT p.id,
		#(p.likes+p.comments)/p.follower as popularity,
		count(m.name) as matches
        FROM photos as p
        LEFT JOIN mtags as m ON m.photoId =  p.id
        WHERE m.`name` = 'group' OR m.`name` = 'festival' OR m.`name` = 'people'
        GROUP BY p.id
		ORDER BY matches DESC
		LIMIT 100
    #) as sub1 ON p.id = sub1.id 
    #WHERE sub1.id IS NOT NULL AND m.name NOT LIKE 'no %'
    #GROUP BY p.id
    #ORDER BY relationQuality DESC
    #LIMIT 100
) as sub2 ON sub2.id = rel.photoId
WHERE sub2.id IS NOT NULL
GROUP by i.name
ORDER by sum(matches) DESC
LIMIT 20
```

notice: 
- [obsolete] die ```3``` die da drinsteht ist die gesamte Anzahl der MTags, die man im WHERE des innersten SELECT abfragt. diese Zahl würde später als c# variable da reingebaut werden
- die ```LIMIT 100``` kann variable einstellt werden. Es ist die Anzahl der Bilder, deren ITags untersucht nach Relevanz werden sollen
- die ```LIMIT 20``` am Ende sind die Anzahl der tags, die man zurückbekommen möchte
- die auskommentierten Sachen können einkommentiert werden zum debuggen