using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vista.Site.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string FeedId { get; set; }
        public string URL { get; set; }
        public string ImageTitle { get; set; }
        public string Link { get; set; }        
    }
}