# current Crawler V1

## Structure:

(1)
  * Start with a specific hashtag or collect random hashtags from 3rd party service
  * collect these hashtags in a HashtagQueue

(2)
  * Crawl "Explore/Tags" Page (https://www.instagram.com/explore/tags/{hashtag}/)
  * get 9 images from Top-Rated category
  * check Conditions MinLikes, MinHashtagsCount
  * collect returning images in ShortcodeQueue

(3)
  * Crawl Image-Detailpage and get username 
  * collect userName in UserQueue

(4)
  * Crawl user-page
  * Get all 12 images
  * check Conditions MinLikes, MinHashtagsCount, MinFollowerCount
  * get imageLink with likes and commentCount from userpage
  * instagram hashtags toLower()
  * do database save