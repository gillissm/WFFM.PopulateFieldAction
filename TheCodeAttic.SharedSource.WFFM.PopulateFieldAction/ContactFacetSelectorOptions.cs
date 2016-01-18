using Sitecore;
using Sitecore.Text;
using Sitecore.Web;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class ContactFacetSelectorOptions
    {
        public string ButtonText { get; set; }

        public string Icon { get; set; }

        public string SelectedPropertyKey { get; set; }

        public string Text { get; set; }

        public string Title { get; set; }

        public string Parameters { get; set; }

        public string DataSourceTypeName { get; set; }

        public static ContactFacetSelectorOptions Parse()
        {
            return new ContactFacetSelectorOptions()
            {
                ButtonText = WebUtil.GetQueryString("bt", "OK"),
                DataSourceTypeName = WebUtil.GetQueryString("ds"),
                Icon = WebUtil.GetQueryString("ic"),
                SelectedPropertyKey = WebUtil.GetQueryString("spk"),
                Text = WebUtil.GetQueryString("txt"),
                Title = WebUtil.GetQueryString("ti"),
                Parameters = WebUtil.GetQueryString("pm")
            };
        }

        public virtual UrlString ToUrlString()
        {
            UrlString urlString = new UrlString(Context.Site.XmlControlPage);
            urlString["xmlcontrol"] = "/sitecore/shell/~/xaml/Sitecore.Forms.Shell.UI.Dialogs.UpdateContactDetails.xaml.xml";
            //urlString["xmlcontrol"] = typeof(ContactFacetSelectorOptions).FullName;
            if (!string.IsNullOrEmpty(this.SelectedPropertyKey))
                urlString.Add("spk", this.SelectedPropertyKey);
            if (!string.IsNullOrEmpty(this.Icon))
                urlString.Add("ic", this.Icon);
            if (!string.IsNullOrEmpty(this.Text))
                urlString.Add("txt", this.Text);
            if (!string.IsNullOrEmpty(this.Title))
                urlString.Add("ti", this.Title);
            if (!string.IsNullOrEmpty(this.ButtonText))
                urlString.Add("bt", this.ButtonText);
            //if (!string.IsNullOrEmpty(this.DataSourceTypeName))
            //    urlString.Add("ds", this.DataSourceTypeName);
            //if (!string.IsNullOrEmpty(this.Parameters))
            //    urlString.Add("pm", this.Parameters);
            return urlString;
        }
    }
}