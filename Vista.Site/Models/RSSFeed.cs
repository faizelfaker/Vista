using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vista.Site.Models
{
    public class RSSFeed
    {
        public string RSSFeedGuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string ImageURL { get; set; }
        public string Generator { get; set; }
        public string LastBuildDate { get; set; }
        public int TTL { get; set; }
        public string Copyright { get; set; }
        public string Language { get; set; }

        public List<Item> RSSItems { get; set; }
    }
}