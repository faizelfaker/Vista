using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Syndication;
using System.Xml;
using Vista.Site.Models;

namespace Vista.Site
{
    public class RSSFeedManager
    {
        public RSSFeedManager()
        {

        }

        /// <summary>
        /// Checks for new RSS Feed Data and loads into DB
        /// </summary>
        /// <param name="strFeedUrl"></param>
        public void LoadRSSFeedData(string strFeedUrl)
        {            
            var rssDataManager = new RSSDataManager();
            
            //Get the RSS Data from the RSSFeed
            var rssRawFeed = GetRSSFeed(strFeedUrl);

            //Check if rssfeed and items exists in DB. 
            //If not, then insert it into DB
            foreach (SyndicationItem item in rssRawFeed.Items)
            {
                //Build item model
                var validateItem = HydrateItem(item, rssRawFeed);      
                
                //Check if the item already exists in the DB
                var isNewItem = rssDataManager.IsNewItem(validateItem);

                //If the item is NOT in the DB, then insert the Item
                if (isNewItem)
                {
                    //Check if it is a new RSSFeed
                    var rssFeedGuid = rssDataManager.IsNewRSSFeed(validateItem);

                    if (rssFeedGuid == "NULL")
                    {   
                        //Insert new RSS feed
                        validateItem.RSSFeedGuid = Guid.NewGuid().ToString();
                        rssDataManager.InsertNewRSSFeed(validateItem);
                    }
                    else
                    {
                        validateItem.RSSFeedGuid = rssFeedGuid;
                    }

                    //Insert the new Item
                    rssDataManager.InsertNewItem(validateItem);
                }
            }            
        }

        /// <summary>
        /// Builds the Item model 
        /// </summary>
        /// <param name="rawItem"></param>
        /// <param name="rssRawFeed"></param>
        /// <returns></returns>
        private Item HydrateItem(SyndicationItem rawItem, SyndicationFeed rssRawFeed)
        {
            var item = new Item
            {
                Title = rssRawFeed.Title.Text,
                Description = rssRawFeed.Description.Text,
                LastBuildDate = rssRawFeed.LastUpdatedTime.ToString(),
                PublishedDate = rawItem.PublishDate.ToString(),
                Link = rssRawFeed.Links[0].Uri.ToString(),
                TTL = 5,
                Copyright = rssRawFeed.Copyright.Text,
                Language = rssRawFeed.Language[0].ToString(),
                ItemTitle = rawItem.Title.Text,
                ItemDescription = rawItem.Summary.Text,
                ItemLink = rawItem.Links[0].Uri.ToString(),
                Guid = rawItem.Id.ToString(),
                ImageURL = rssRawFeed.ImageUrl.ToString(),
                RSSItems = new List<Item>() 
            };

            item.RSSItems.Add(item);

            return item;
        }


        private bool ValidateItem(SyndicationItem validItem)
        {
            return true;
        }

        public SyndicationFeed GetRSSFeed(string strFeedUrl)
        {
            string url = strFeedUrl;
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed rssRawFeed = SyndicationFeed.Load(reader);
            reader.Close();

            return rssRawFeed;
        }
    }
}