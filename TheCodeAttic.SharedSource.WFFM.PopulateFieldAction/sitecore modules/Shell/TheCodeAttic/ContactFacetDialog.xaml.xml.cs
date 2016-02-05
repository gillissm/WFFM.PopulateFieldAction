
using Sitecore.Analytics.Model.Framework;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Data.Helpers;
using Sitecore.Forms.Shell.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Utilities;

using System.Linq;
using System.Web.UI.HtmlControls;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Xaml;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Sheer
{
    //https://briancaos.wordpress.com/2010/11/03/xaml-sheerui-dialog-in-sitecore-6-3-x/
    public class ContactFacetDialog : EditorBase
    {
        // Define each control that you wish to
        // programatically alter the contents or
        // behaviour of
        //protected Literal Name;
        //protected Scrollbox Summary;
        protected TreeView myTreeView;
        protected HtmlInputHidden MappedFields;

        

        protected override void OnInit(EventArgs e)
        {
            this.MappedFields.Value = this.GetValueByKey("spk");
            base.OnInit(e);
        }
        protected override void SaveValues()
        {
            this.MappedFields.Value = myTreeView.SelectedValue;
        }



        protected override void OK_Click()
        {
            this.SaveValues();
            XamlControl.AjaxScriptManager.SetDialogValue(this.MappedFields.Value);
            SheerResponse.CloseWindow();
        }



        protected override void Localize()
        {
            this.Header = WebUtil.GetQueryString("ti");
            this.Text = WebUtil.GetQueryString("txt");         
        }

     

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnLoad(e);
            //if (!Context.ClientPage.IsEvent)            
            //{
            if (!Page.IsPostBack) { 
                myTreeView.Nodes.Clear();

                Dictionary<string, IFacet> cf = ContactFacetHelper.FacetFactory.ContactFacets;
                foreach (KeyValuePair<string, IFacet> f in cf)
                {
                    myTreeView.Nodes.Add(GetTreeNode(f.Key, f.Value, f.Key));
                }
                myTreeView.DataBind();
    
            }
            //else
            //{
            //    MappedFields.Value = myTreeView.SelectedValue;
            //}
        }

        /// <summary>
        /// Type: Sitecore.Forms.Core.Data.Helpers.ContactFacetsHelper
        /// Assembly: Sitecore.Forms.Core, Version=8.1.0.0, Culture=neutral, PublicKeyToken=null
        /// Type: Sitecore.WFFM.Analytics.ContactFacetFactory
        /// Assembly: Sitecore.WFFM.Analytics, Version=8.1.0.0, Culture=neutral, PublicKeyToken=null
        /// </summary>
        /// <param name="name"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private TreeNode GetTreeNode(string name, IElement f, string path)
        {
            TreeNode tn = new TreeNode();

            IEnumerable<IModelMember> facetMembers = ContactFacetsHelper.FacetFactory.GetFacetMembers(f);
            
            tn.Text= name;
            tn.Value = path;
            if(facetMembers!= null && facetMembers.Any())
            {
                foreach (IModelMember modelMember in facetMembers)
                {                    
                    if (modelMember is IModelAttributeMember && !string.Equals(modelMember.Name, "Preferred", StringComparison.OrdinalIgnoreCase))
                    {
                         tn.ChildNodes.Add(new TreeNode(modelMember.Name,string.Format("{0}/{1}", path,modelMember.Name)));                        
                    }
                    else if (modelMember is IModelElementMember)
                    {
                        IModelElementMember modelElementMember = modelMember as IModelElementMember;
                        tn.ChildNodes.Add(GetTreeNode(modelElementMember.Name, modelElementMember.Element, string.Format("{0}/{1}", path, modelElementMember.Name)));
                    }
                    else if (modelMember is IModelDictionaryMember || modelMember is IModelCollectionMember)
                    {
                        IModelDictionaryMember dictionaryMember = modelMember as IModelDictionaryMember;
                        Type type = dictionaryMember == null ? Enumerable.Single<Type>((IEnumerable<Type>)(modelMember as IModelCollectionMember).Elements.GetType().GetGenericArguments()) : Enumerable.Single<Type>((IEnumerable<Type>)dictionaryMember.Elements.GetType().GetGenericArguments());
                        if (!(type == (Type)null))
                            tn.ChildNodes.Add(GetTreeNode("Entries", ContactFacetsHelper.FacetFactory.CreateElement(type), string.Format("{0}/{1}", path, "Entries")));
                    }                       
                    
                }
                
            }
            return tn;
        }
        
        //protected override void Cancel_Click()
        //{
        //    string e = "hi cancel";
        //}
        //protected override void OK_Click()
        //{
        //    string z = "hi";

        //}

        //private Item GetCurrentItem()
        //{
        //     Parse the querystring to get the item
        //    string queryString = WebUtil.GetQueryString("id");
        //    Language language = Language.Parse(WebUtil.GetQueryString("la"));
        //    Version version = Version.Parse(WebUtil.GetQueryString("vs"));
        //    return Client.ContentDatabase.GetItem(queryString, language, version);
        //}

        //private void CreateFacetTree()
        //{
        //    XElement parent = new XElement((XName)"Top");
        //    ContactFacetsHelper.CreateNode(parent, "facets", true, "");
        //    parent.Add((object)new XElement((XName)"isFolder", (object)true));
        //    parent.Add((object)new XElement((XName)"expand", (object)true));
        //    foreach (KeyValuePair<string, IFacet> keyValuePair in ContactFacetsHelper.FacetFactory.ContactFacets)
        //    {
        //        XElement node = new XElement((XName)"children");
        //        parent.Add((object)node);
        //        ContactFacetsHelper.ExpandElement(node, (IElement)keyValuePair.Value, keyValuePair.Key, "");
        //    }
        //}

    }
    

    /*
    public class ContactFacetDialog : EditorBase
    {

      
    private const string MappingKey = "Mapping";
    protected PlaceHolder Content;
    protected Checkbox OverwriteContact;
    protected HtmlInputHidden MappedFields;
    protected Groupbox UserProfileGroupbox;
    private string FormFieldColumnHeader;
    private string ContactDetailsColumnHeader;

    public string RestrictedFieldTypes
    {
      get
      {
        return WebUtil.GetQueryString("RestrictedFieldTypes", "{1F09D460-200C-4C94-9673-488667FF75D1}|{1AD5CA6E-8A92-49F0-889C-D082F2849FBD}|{7FB270BE-FEFC-49C3-8CB4-947878C099E5}");
      }
    }

    protected override void OnInit(EventArgs e)
    {
      this.MappedFields.Value = this.GetValueByKey("Mapping");
      base.OnInit(e);
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      //if (Context.ClientPage.IsEvent)
      //  return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" if (typeof ($scw) === \"undefined\") {");
      stringBuilder.Append(" window.$scw = jQuery.noConflict(true); }");
      stringBuilder.Append(" $scw(document).ready(function () {");
      stringBuilder.Append(string.Format(" var treeData = $scw.parseJSON('{0}');", (object) ContactFacetsHelper.GetContactFacetsXmlTree()));
      stringBuilder.Append(string.Format(" var fieldsData = $scw.parseJSON('{0}');", (object) this.GetFieldsData(this.RestrictedFieldTypes)));
      stringBuilder.Append(string.Format(" var selectedDataValue = $scw(\"#{0}\").val();", (object) this.MappedFields.ClientID));
      stringBuilder.Append(" var selectedData = [];");
      stringBuilder.Append(" if(selectedDataValue) {");
      stringBuilder.Append(" selectedData = $scw.parseJSON(selectedDataValue); }");
      stringBuilder.Append(" $scw(\"#treeMap\").droptreemap({");
      stringBuilder.Append(" treeData: treeData.Top,");
      stringBuilder.Append(" selected: selectedData,");
      stringBuilder.Append(" listData: fieldsData,");
      stringBuilder.Append(string.Format(" fieldsHeader: \"{0}\",", (object) this.FormFieldColumnHeader));
      stringBuilder.Append(string.Format(" mappedKeysHeader: \"{0}\",", (object) this.ContactDetailsColumnHeader));
      stringBuilder.Append(string.Format(" addFieldTitle: \"{0}\",", (object) DependenciesManager.ResourceManager.Localize("ADD_FIELD")));
      stringBuilder.Append(" change: function (value) {");
      stringBuilder.Append(string.Format(" $scw(\"#{0}\").val(value);", (object) this.MappedFields.ClientID));
      stringBuilder.Append("} });   });");
      this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sc_wffm_update_contact" + this.ClientID, stringBuilder.ToString(), true);
    }

    protected string GetFieldsData(string restrictedTypes = "")
    {
      return "[" + string.Join(",", Enumerable.Select<KeyValuePair<string, string>, string>((IEnumerable<KeyValuePair<string, string>>) Enumerable.ToDictionary<IFieldItem, string, string>(Enumerable.Where<IFieldItem>((IEnumerable<IFieldItem>) new FormItem(this.CurrentDatabase.GetItem(this.CurrentID)).Fields, (Func<IFieldItem, bool>) (property =>
      {
        if (!string.IsNullOrEmpty(restrictedTypes))
          return !restrictedTypes.Contains(property.TypeID.ToString());
        return true;
      })), (Func<IFieldItem, string>) (property => property.ID.ToString()), (Func<IFieldItem, string>) (property => property.Name)), (Func<KeyValuePair<string, string>, string>) (d => "{\"id\":\"" + d.Key.Trim('{', '}') + "\",\"title\":\"" + d.Value + "\"}"))) + "]";
    }

    protected override void Localize()
    {
      this.Header = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_HEADER");
      this.Text = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_DESCRIPTION");
      this.FormFieldColumnHeader = DependenciesManager.ResourceManager.Localize("FORM_FIELD");
      this.ContactDetailsColumnHeader = DependenciesManager.ResourceManager.Localize("CONTACT_DETAILS");
    }

    protected override void SaveValues()
    {
      base.SaveValues();
      this.SetValue("Mapping", this.MappedFields.Value);
    }


        ////protected HtmlInputHidden MappedFields;
        
        ////private string FormFieldColumnHeader;
        ////private string ContactDetailsColumnHeader;
        ////protected TreeView tv;

        ////protected override void OnLoad(EventArgs e)
        ////{
        ////    Assert.ArgumentNotNull(e, "e");
        ////    base.OnLoad(e);
        ////    tv.DataSource = JsonConvert.DeserializeObject(ContactFacetsHelper.GetContactFacetsXmlTree());
        ////    tv.DataBind();

        ////    //StringBuilder stringBuilder = new StringBuilder();
        ////    //stringBuilder.Append(" if (typeof ($scw) === \"undefined\") {");
        ////    //stringBuilder.Append(" window.$scw = jQuery.noConflict(true); }");
        ////    //stringBuilder.Append(" $scw(document).ready(function () {");
        ////    //stringBuilder.Append("alert('hello world');");
        ////    //stringBuilder.Append(string.Format(" var treeData = $scw.parseJSON('{0}');", (object)ContactFacetsHelper.GetContactFacetsXmlTree()));
        ////    //stringBuilder.Append(string.Format(" var selectedDataValue = $scw(\"#{0}\").val();", (object)this.MappedFields.ClientID));
        ////    //stringBuilder.Append(" var selectedData = [];");
        ////    //stringBuilder.Append(" if(selectedDataValue) {");
        ////    //stringBuilder.Append(" selectedData = $scw.parseJSON(selectedDataValue); }");
        ////    //stringBuilder.Append(" $scw(\"#treeMap\").droptreemap({");
        ////    //stringBuilder.Append(" treeData: treeData.Top,");
        ////    //stringBuilder.Append(" selected: selectedData,");
        ////    //stringBuilder.Append(" listData: fieldsData,");
        ////    ////stringBuilder.Append(string.Format(" fieldsHeader: \"{0}\",", "FormFieldColumnHeader"));
        ////    ////stringBuilder.Append(string.Format(" mappedKeysHeader: \"{0}\",", "ContactDetailsColumnHeader"));
        ////    ////stringBuilder.Append(string.Format(" addFieldTitle: \"{0}\",", "ADD_FIELD"));

        ////    //stringBuilder.Append(string.Format(" fieldsHeader: \"{0}\",", (object)this.FormFieldColumnHeader));
        ////    //stringBuilder.Append(string.Format(" mappedKeysHeader: \"{0}\",", (object)this.ContactDetailsColumnHeader));
        ////    //stringBuilder.Append(string.Format(" addFieldTitle: \"{0}\",", (object)DependenciesManager.ResourceManager.Localize("ADD_FIELD")));

        ////    //stringBuilder.Append(" change: function (value) {");
        ////    //stringBuilder.Append(string.Format(" $scw(\"#{0}\").val(value);", (object)this.MappedFields.ClientID));
        ////    //stringBuilder.Append("} });");

        ////    //stringBuilder.Append("$scw('#mytree').dynatree({");
        ////    //stringBuilder.Append("children: treeData");
        ////    //stringBuilder.Append(" });");


        ////    //stringBuilder.Append(" });");//end ready
        ////    //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "sc_wffm_update_contact" + this.ClientID, stringBuilder.ToString(), true);
        ////    ////customscript.Text = stringBuilder.ToString();
        ////}

        ////protected override void SaveValues()
        ////{
        ////    base.SaveValues();
        ////    this.SetValue("facefield", this.MappedFields.Value);
        ////}

        ////protected override void Localize()
        ////{
        ////    this.Header = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_HEADER");
        ////    this.Text = DependenciesManager.ResourceManager.Localize("UPDATE_CONTACT_DESCRIPTION");
        ////    this.FormFieldColumnHeader = DependenciesManager.ResourceManager.Localize("FORM_FIELD");
        ////    this.ContactDetailsColumnHeader = DependenciesManager.ResourceManager.Localize("CONTACT_DETAILS");
        ////}
    }
     */
}