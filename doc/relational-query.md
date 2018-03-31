# A query for a relational DB based on better-batabase.md

```
SELECT
#i.id,
#i.photoId,
i.value,
#relationQuality,
#count(i.value)
FROM itags as i
LEFT JOIN
(
    SELECT p.id,
    #p.popularity,
    #matches,
    #count(m.value)+3-matches as overall,
    #count(m.value)-2*matches+3 as missing,
    #[missing]/[overall]
    #(count(m.value)-2*matches+3) / (count(m.value)+3-matches) as searchQuality,
    #[searchQuality]/[popularity]
    ((count(m.value)-2*matches+3) / (count(m.value)+3-matches)) * p.popularity as relationQuality
    FROM photos as p
    LEFT JOIN mtags as m ON m.photoId =  p.id
    LEFT JOIN
    (
        SELECT p.id, p.popularity, count(m.value) as matches
        FROM photos as p
        LEFT JOIN mtags as m ON m.photoId =  p.id
        WHERE m.`value` = 'M3' OR m.`value` = 'M4' OR m.`value` = 'M123'
        GROUP by p.id
    ) as sub1 ON p.id = sub1.id 
    WHERE sub1.id IS NOT NULL
    GROUP by p.id
    ORDER by relationQuality DESC
    LIMIT 10
) as sub2 ON sub2.id = i.photoId
WHERE sub2.id IS NOT NULL
GROUP by i.value
ORDER by count(i.value) DESC, relationQuality DESC
```
notice: 
- die ```3``` die da drinsteht ist die gesamte Anzahl der Tags, die man abfragt. diese würde später als c# variable da reingebaut werden
- die auskommentierten Sachen können einkommentiert werden zum debuggen