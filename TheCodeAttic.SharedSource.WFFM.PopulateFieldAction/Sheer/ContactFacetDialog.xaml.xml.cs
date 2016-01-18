using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Data.Helpers;
using Sitecore.Forms.Shell.UI.Dialogs;
using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Routing;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;
using System.Web.Util;
using Sitecore.WFFM.Abstractions;


namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Sheer
{
    public class ContactFacetDialog : EditorBase
    {
        protected HtmlInputHidden MappedFields;
        protected Literal customscript;
        private string FormFieldColumnHeader;
        private string ContactDetailsColumnHeader;

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" if (typeof ($scw) === \"undefined\") {");
            stringBuilder.Append(" window.$scw = jQuery.noConflict(true); }");
            stringBuilder.Append(" $scw(document).ready(function () {");
            stringBuilder.Append("alert('hello world');");
            stringBuilder.Append(string.Format(" var treeData = $scw.parseJSON('{0}');", (object)ContactFacetsHelper.GetContactFacetsXmlTree()));
            stringBuilder.Append(string.Format(" var selectedDataValue = $scw(\"#{0}\").val();", (object)this.MappedFields.ClientID));
            stringBuilder.Append(" var selectedData = [];");
            stringBuilder.Append(" if(selectedDataValue) {");
            stringBuilder.Append(" selectedData = $scw.parseJSON(selectedDataValue); }");
            stringBuilder.Append(" $scw(\"#treeMap\").droptreemap({");
            stringBuilder.Append(" treeData: treeData.Top,");
            stringBuilder.Append(" selected: selectedData,");
            stringBuilder.Append(" listData: fieldsData,");
            //stringBuilder.Append(string.Format(" fieldsHeader: \"{0}\",", "FormFieldColumnHeader"));
            //stringBuilder.Append(string.Format(" mappedKeysHeader: \"{0}\",", "ContactDetailsColumnHeader"));
            //stringBuilder.Append(string.Format(" addFieldTitle: \"{0}\",", "ADD_FIELD"));

            stringBuilder.Append(string.Format(" fieldsHeader: \"{0}\",", (object)this.FormFieldColumnHeader));
            stringBuilder.Append(string.Format(" mappedKeysHeader: \"{0}\",", (object)this.ContactDetailsColumnHeader));
            stringBuilder.Append(string.Format(" addFieldTitle: \"{0}\",", (object)DependenciesManager.ResourceManager.Localize("ADD_FIELD")));

            stringBuilder.Append(" change: function (value) {");
            stringBuilder.Append(string.Format(" $scw(\"#{0}\").val(value);", (object)this.MappedFields.ClientID));
            stringBuilder.Append("} });");

            stringBuilder.Append("$scw('#mytree').dynatree({");
            stringBuilder.Append("children: treeData");
            stringBuilder.Append(" });");


            stringBuilder.Append(" });");//end ready
            //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sc_wffm_update_contact" + this.ClientID, stringBuilder.ToString(), true);
            customscript.Text = stringBuilder.ToString();
        }

        protected override void SaveValues()
        {
            base.SaveValues();
            this.SetValue("facefield", this.MappedFields.Value);
        }

        protected override void Localize()
        {
            this.Header = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_HEADER");
            this.Text = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_DESCRIPTION");
            this.FormFieldColumnHeader = DependenciesManager.ResourceManager.Localize("FORM_FIELD");
            this.ContactDetailsColumnHeader = DependenciesManager.ResourceManager.Localize("CONTACT_DETAILS");
        }
    }
}