

# current Crawler V1

## Structure:

(1)
  * Start with a specific hashtag or collect random hashtags from 3rd party service
  * collect these hashtags in a HashtagQueue

(2)
  * Open "Explore/Tags" Page (https://www.instagram.com/explore/tags/{hashtag}/)
  * get 9 images from Top-Rated category
  * check Conditions MinLikes, MinHashtagsCount
  * collect returning images in ShortcodeQueue

(3)
  * Obsolete: Crawle dieses Fotos 
  * ToDo: UserQueue
  
  * Stattdessen: CrawlerV2 Logik, um alle Image hiervon zu kriegen und abzuspeichern
  * Optional: Finde large-Photo-URL heraus von user-Seite aus (statt small image zu nehmen)