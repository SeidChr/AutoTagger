using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTagger.Crawler.Standard.V1.Crawler
{
    using System.Linq;

    public class RandomTagsCrawler : HttpCrawler
    {
        public IEnumerable<string> Parse()
        {
            // https://top-hashtags.com/random/
            // https://www.all-hashtag.com/library/contents/ajax_top.php
            var document = this.FetchDocument("https://www.all-hashtag.com/library/contents/ajax_top.php");

            // <section id="tab1" class="tab"><h4 class="tab-title">Top 100 hashtags <span class="color-brand">today</span></h4><span class="hashtag">#look</span>
            var nodes = document.SelectNodes("//section[@id='tab1']//span[@class='hashtag']");
            return nodes.Select(n => n.InnerText.Trim(' ', '#'));
        }
    }
}
