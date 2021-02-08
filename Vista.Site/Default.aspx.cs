using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;
using Vista.Site;
using System.Configuration;
using System.ServiceModel.Syndication;
using Vista.Site.Models;
using System.Data;
using System.Web.Services;

namespace Vista.Site
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!Page.IsPostBack)
            {                
                this.BindGrid();
            }
        }

        private void BindGrid()
        {
            List<RSSFeed> rssDataManader = GetRSSFeeds();

            lblRSSDescription.Text = rssDataManader[0].Description;
            lblRSSTitle.Text = rssDataManader[0].Title;
            imgRSSImage.ImageUrl = rssDataManader[0].ImageURL;

            grvRSSFeed.DataSource = rssDataManader[0].RSSItems;
            grvRSSFeed.DataBind();
                        
            grvRSSFeed.UseAccessibleHeader = true;
            grvRSSFeed.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        
        public  List<RSSFeed> GetRSSFeeds()
        {
            var rssDataManader = new RSSDataManager();
            return rssDataManader.GetRSSFeedsAndItems();
        }
    }
}