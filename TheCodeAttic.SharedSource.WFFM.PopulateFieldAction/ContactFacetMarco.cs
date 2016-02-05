using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Rules.RuleMacros;
//using Sitecore.Social.Client.Dialogs.SocialPropertiesSelectors;
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

            UrlString str = new UrlString("/sitecore/shell/~/xaml/ContactFacetDialog.aspx?ti=Contact Facet Picker&txt=Select a contact facet property&spk="+value);                        
            Sitecore.Web.UI.HtmlControls.ClientCommand cc = SheerResponse.ShowModalDialog(str.ToString(),true);
        }
    }
}