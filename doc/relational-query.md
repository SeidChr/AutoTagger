# A query for a relational DB based on better-batabase.md

```
SELECT
i.id as itagId,
i.name,
relationQuality,
count(i.name) as imagesCount
FROM itags as i
LEFT JOIN photo_itag_rel as rel ON rel.itagId = i.id
LEFT JOIN
(
    SELECT p.id,
    #popularity,
    #matches,
    #count(m.name)+3-matches as overall,
    #count(m.name)-2*matches+3 as missing,
    #[missing]/[overall]
    #(count(m.name)-2*matches+3) / (count(m.name)+3-matches) as searchQuality,
    #[searchQuality]/[popularity]
    ((count(m.name)-2*matches+3) / (count(m.name)+3-matches)) * popularity as relationQuality
    FROM photos as p
    LEFT JOIN mtags as m ON m.photoId = p.id
    LEFT JOIN
    (
        SELECT p.id, (p.likes+p.comments)/p.follower as popularity, count(m.name) as matches
        FROM photos as p
        LEFT JOIN mtags as m ON m.photoId =  p.id
        WHERE m.`name` = 'group' OR m.`name` = 'festival' OR m.`name` = 'people'
        GROUP by p.id
		ORDER by matches DESC
		LIMIT 200
    ) as sub1 ON p.id = sub1.id 
    WHERE sub1.id IS NOT NULL
    GROUP by p.id
    ORDER by relationQuality DESC
    LIMIT 200
) as sub2 ON sub2.id = rel.photoId
WHERE sub2.id IS NOT NULL
GROUP by i.name
ORDER by count(i.name) DESC, relationQuality DESC
LIMIT 30
```

notice: 
- die ```3``` die da drinsteht ist die gesamte Anzahl der MTags, die man im WHERE des innersten SELECT abfragt. diese Zahl würde später als c# variable da reingebaut werden
- die ```LIMIT 200``` kann variable einstellt werden. Es ist die Anzahl der Bilder, deren ITags untersucht nach Relevanz werden sollen
- die ```LIMIT 30``` am Ende sind die Anzahl der tags, die man zurückbekommen möchte
- die auskommentierten Sachen können einkommentiert werden zum debuggen