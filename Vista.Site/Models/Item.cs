using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vista.Site.Models
{
    public class Item : RSSFeed
    {
        public int ItemId { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }
        public string ItemLink { get; set; }
        public string Guid { get; set; }
        public string PublishedDate { get; set; }
    }
}