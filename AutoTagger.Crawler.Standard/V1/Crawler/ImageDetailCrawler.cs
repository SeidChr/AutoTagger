namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using AutoTagger.Crawler.Standard.Helper;

    public class ImageDetailCrawler : HttpCrawler
    {

        public string Parse(string url)
        {
            var document = this.FetchDocument(url);
            var scriptNode = this.GetScriptNodeData(document);
            var userName = GetImageNode(scriptNode);
            return userName;
        }

        private static dynamic GetImageNode(dynamic data)
        {
            return data
                ?.entry_data
                ?.PostPage?[0]
                ?.graphql
                ?.shortcode_media
                ?.owner
                ?.username
                .ToString();
        }
    }
}
