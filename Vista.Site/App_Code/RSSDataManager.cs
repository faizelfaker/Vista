using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using System.Configuration;
using Vista.Site.Models;
using System.Text;

namespace Vista.Site
{
    public class RSSDataManager
    {
        /// <summary>
        /// Create a connection instance of the SQLite database
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection DBInstance()
        {
            var strVistaConnection = ConfigurationManager.AppSettings["dbVistaLive"];

            var conn = new SQLiteConnection(@"Data Source=" + strVistaConnection);
            conn.Open();

            return conn;
        }


        /// <summary>
        /// Check if the Item has already been inserted in the DB
        /// </summary>
        /// <param name="item"></param>
        /// <returns>boolean</returns>
        public bool IsNewItem(Item item)
        {
            bool isNewItem;

            using (SQLiteConnection conn = DBInstance())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM Items WHERE Guid = '" + item.Guid + "'";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        //Check if the item was found in the database
                        if (reader.HasRows)
                        {
                            //This is not a new item                    
                            isNewItem = false;
                        }
                        else
                        {
                            //This is a new item
                            isNewItem = true;
                        }

                        reader.Close();
                    }

                    cmd.Dispose();
                }

                conn.Close();
                conn.Dispose();
            }

            return isNewItem;
        }


        /// <summary>
        /// Check if the RSSFeed has already been inserted in the DB
        /// </summary>
        /// <param name="item"></param>
        /// <returns>string</returns>
        public string IsNewRSSFeed(Item item)
        {
            string rssFeedGuid = "NULL";

            using (SQLiteConnection conn = DBInstance())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM RSSFeed WHERE Copyright = '" + item.Copyright + "'";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        //Check if the item was found in the database
                        if (reader.HasRows)
                        {
                            //This is not a new item
                            while (reader.Read())
                            {
                                rssFeedGuid = reader["RSSFeedGuid"].ToString();
                            }
                        }
                        else
                        {
                            //This is a new item
                            rssFeedGuid = "NULL";
                        }

                        reader.Close();
                    }

                    cmd.Dispose();
                }

                conn.Close();
                conn.Dispose();
            }

            return rssFeedGuid;
        }


        /// <summary>
        /// Inserts a new RSSFeed into the database
        /// </summary>
        /// <param name="item"></param>
        public void InsertNewRSSFeed(Item item)
        {
            using (SQLiteConnection conn = DBInstance())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    StringBuilder sbComm = new StringBuilder();

                    sbComm.Append("INSERT INTO RSSFeed ");
                    sbComm.Append("(RSSFeedGuid, Title, Description, Link, Generator, LastBuildDate, Ttl, Copyright, Language, ImageURL)");
                    sbComm.Append(" VALUES ");
                    sbComm.Append("('" + item.RSSFeedGuid + "', '" + item.Title.Replace("'", "''") + "', '" + item.Description.Replace("'", "''") + "', '" + item.Link + "', '" + item.Generator + "', ");
                    sbComm.Append("'" + item.LastBuildDate + "', 5, '" + item.Copyright + "', '" + item.Language + "', '" + item.ImageURL + "')");

                    cmd.CommandText = sbComm.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
        }


        /// <summary>
        /// Inserts a new Item into the database
        /// </summary>
        /// <param name="item"></param>
        public void InsertNewItem(Item item)
        {
            using (SQLiteConnection conn = DBInstance())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    StringBuilder sbComm = new StringBuilder();

                    sbComm.Append("INSERT INTO Items ");
                    sbComm.Append("(RSSFeedGuid, Title, Description, Link, Guid, PubDate)");
                    sbComm.Append(" VALUES ");
                    sbComm.Append("('" + item.RSSFeedGuid + "', '" + item.ItemTitle.Replace("'", "''") + "', '" + item.ItemDescription.Replace("'", "''") + "', '" + item.ItemLink + "', '" + item.Guid + "', '" + item.PublishedDate + "')");

                    cmd.CommandText = sbComm.ToString();

                    cmd.ExecuteScalar();
                    cmd.Dispose();
                }

                conn.Close();
                conn.Dispose();
            }
        }


        /// <summary>
        /// Gets RSSFeed and Items
        /// </summary>
        /// <param name="item"></param>
        /// <returns>boolean</returns>
        public List<RSSFeed> GetRSSFeedsAndItems()
        {
            var rssFeeds = new List<RSSFeed>();

            using (SQLiteConnection conn = DBInstance())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT * FROM RSSFeed";

                    //Get all RSSFeeds
                    using (SQLiteDataReader sdrRSSFeeds = cmd.ExecuteReader())
                    {
                        if (sdrRSSFeeds.HasRows)
                        {
                            while (sdrRSSFeeds.Read())
                            {
                                var rssFeed = new RSSFeed();
                                rssFeed.RSSFeedGuid = sdrRSSFeeds["RSSFeedGuid"].ToString();
                                rssFeed.Title = sdrRSSFeeds["Title"].ToString();
                                rssFeed.Copyright = sdrRSSFeeds["Copyright"].ToString();
                                rssFeed.ImageURL = sdrRSSFeeds["ImageURL"].ToString();
                                rssFeed.Link = sdrRSSFeeds["Link"].ToString();
                                rssFeed.LastBuildDate = sdrRSSFeeds["LastBuildDate"].ToString();
                                rssFeed.TTL = Convert.ToInt32(sdrRSSFeeds["Ttl"]);
                                rssFeed.Generator = sdrRSSFeeds["Generator"].ToString();
                                rssFeed.Description = sdrRSSFeeds["Description"].ToString();
                                rssFeed.RSSItems = new List<Item>();

                                //Get RSSFeed items
                                rssFeeds.Add(GetFeedItems(rssFeed));                          
                            }

                            sdrRSSFeeds.Close();
                        }

                        cmd.Dispose();
                    }

                    conn.Close();
                    conn.Dispose();
                }

                return rssFeeds;
            }
        }


        /// <summary>
        /// Gets RSSFeed Items
        /// </summary>
        /// <param name="rssFeed"></param>
        /// <returns>RSSFeed</returns>
        public RSSFeed GetFeedItems(RSSFeed rssFeed)
        {
            //Get the items for the RSSFeeds
            using (SQLiteConnection conn2 = DBInstance())
            {
                using (SQLiteCommand cmd2 = new SQLiteCommand(conn2))
                {
                    cmd2.CommandText = "SELECT * FROM Items WHERE RSSFeedGuid = '" + rssFeed.RSSFeedGuid + "'";

                    using (SQLiteDataReader sdrsItems = cmd2.ExecuteReader())
                    {
                        if (sdrsItems.HasRows)
                        {
                            while (sdrsItems.Read())
                            {
                                var rssItem = new Item();
                                rssItem.RSSFeedGuid = sdrsItems["RSSFeedGuid"].ToString();
                                rssItem.ItemTitle = sdrsItems["Title"].ToString();                                
                                rssItem.ItemLink =  sdrsItems["Link"].ToString();
                                rssItem.ItemDescription = sdrsItems["Description"].ToString();
                                rssItem.PublishedDate = sdrsItems["PubDate"].ToString();
                                rssItem.Guid = sdrsItems["Guid"].ToString();
                                rssItem.Title = rssFeed.Title;
                                rssItem.Copyright = rssFeed.Copyright;
                                rssItem.ImageURL = rssFeed.ImageURL;
                                rssItem.Link = rssFeed.Link;
                                rssItem.LastBuildDate = rssFeed.LastBuildDate;
                                rssItem.TTL = rssFeed.TTL;
                                rssItem.Generator = rssFeed.Generator;
                                rssItem.Description = rssFeed.Description;

                                //Add the item to RSSFeed List
                                rssFeed.RSSItems.Add(rssItem);
                            }
                        }

                        sdrsItems.Close();
                    }

                    cmd2.Dispose();
                    conn2.Close();
                    conn2.Dispose();
                }
            }

            return rssFeed;
        }        
    }
}
