namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    public class ImageDetailCrawler : HttpCrawler
    {

        public string Parse(string url)
        {
            var document = this.FetchDocument(url);
            var scriptNode = GetScriptNodeData(document);
            var userName = GetImageNode(scriptNode);
            return userName;
        }

        private static dynamic GetImageNode(dynamic data)
        {
            if (data == null)
            {
                return null;
            }
            return data.entry_data?.PostPage?[0]?.graphql?.shortcode_media?.owner?.username.ToString();
        }
    }
}
