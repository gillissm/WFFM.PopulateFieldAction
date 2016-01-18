using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Rules.RuleMacros;
using Sitecore.Social.Client.Dialogs.SocialPropertiesSelectors;
//using Sitecore.Social.Client.Dialogs.SocialPropertiesSelectors;
//using Sitecore.Social.Client.Rules.DataSources;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using System.Xml.Linq;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class ContactFacetMarco:IRuleMacro
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">XML of calling condition rule or action</param>
        /// <param name="name">property of the condition/action that will be set by the macro</param>
        /// <param name="parameters">parameter details, the third comma in the text</param>
        /// <param name="value">currntly set value to the property</param>
        public void Execute(XElement element, string name, UrlString parameters, string value)
        {
            Assert.ArgumentNotNull((object)element, "element");
            Assert.ArgumentNotNull((object)name, "name");
            Assert.ArgumentNotNull((object)parameters, "parameters");
            Assert.ArgumentNotNull((object)value, "value");

            //SocialPropertiesSelectorOptions propertiesSelectorOptions = new SocialPropertiesSelectorOptions()
            //{
            //    Title = Translate.Text("Gender"),
            //    Text = Translate.Text("Select the gender that you want to use for this rule."),
            //    Icon = "People/32x32/users2.png",
            //    DataSourceTypeName = typeof(Sitecore.Social.Client.Rules.DataSources.GenderPropertiesDataSource).FullName
            //};


            //if (!string.IsNullOrEmpty(value))
            //    propertiesSelectorOptions.SelectedPropertyKey = value;
            //SheerResponse.ShowModalDialog(propertiesSelectorOptions.ToUrlString().ToString(), true);


            /////sitecore/shell/~/xaml/Sitecore.Forms.Shell.UI.Dialogs.UpdateContactDetails.xaml.xml

            //ContactFacetSelectorOptions selectorOptions = new ContactFacetSelectorOptions()
            //{
            //    Title = "Contact Facet Picker",
            //    Text = "Select the contact facet field to be used in the form.",
            //    Icon = "People/32x32/users2.png",
            //    SelectedPropertyKey = !string.IsNullOrWhiteSpace(value) ? value : string.Empty
            //};

            //SheerResponse.ShowModalDialog(selectorOptions.ToUrlString().ToString(), true);
            UrlString str = new UrlString("/sitecore/shell/~/xaml/TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Sheer.ContactFacetDialog.aspx");
            //str["actionid"] = element.Attribute("id").Value;
            //str["uniqid"] = element.Attribute("uid").Value;
            //str["id"] = "{2808379D-9284-4060-AC87-BE2BFADF93C6}";//Form
            
            SheerResponse.ShowModalDialog(str.ToString(), true);


            //https://community.sitecore.net/developers/f/8/t/1201




        }
    }
}