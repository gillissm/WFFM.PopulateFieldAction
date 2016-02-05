
using Sitecore.Analytics.Model.Framework;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Data.Helpers;
using Sitecore.Forms.Shell.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;

using System.Linq;
using System.Web.UI.HtmlControls;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Xaml;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Sheer
{    
    public class ContactFacetDialog : EditorBase
    {

        protected TreeView myTreeView;
        protected HtmlInputHidden MappedFields;

        protected override void OnInit(EventArgs e)
        {
            this.MappedFields.Value = WebUtil.GetQueryString("spk");
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

            if (!Page.IsPostBack)
            {
                myTreeView.Nodes.Clear();

                Dictionary<string, IFacet> cf = ContactFacetsHelper.FacetFactory.ContactFacets;
                foreach (KeyValuePair<string, IFacet> f in cf)
                {
                    myTreeView.Nodes.Add(GetTreeNode(f.Key, f.Value, f.Key));
                }

                myTreeView.DataBind();

            }
            else if (!string.IsNullOrWhiteSpace(myTreeView.SelectedValue))
            {
                OK_Click();
            }
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

            tn.Text = name;
            tn.Value = path;
            if (facetMembers != null && facetMembers.Any())
            {
                foreach (IModelMember modelMember in facetMembers)
                {
                    if (modelMember is IModelAttributeMember && !string.Equals(modelMember.Name, "Preferred", StringComparison.OrdinalIgnoreCase))
                    {
                        var newItemPath = string.Format("{0}/{1}", path, modelMember.Name);
                        var newNode = new TreeNode(modelMember.Name, newItemPath);
                        newNode.Selected = newItemPath.ToLowerInvariant() == this.MappedFields.Value.ToLowerInvariant();
                        tn.ChildNodes.Add(newNode);
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
    }
}