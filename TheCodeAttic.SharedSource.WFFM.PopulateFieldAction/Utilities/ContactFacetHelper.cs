using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.WFFM.Abstractions;
using Sitecore.WFFM.Abstractions.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.Utilities
{
    public static class ContactFacetHelper
    {

        public static IContactFacetFactory FacetFactory
        {
            get
            {
                return DependenciesManager.FacetFactory.GetContactFacetFactory();
            }
        }

        public static ContactManager ContactManager
        {
            get
            {
                return Sitecore.Configuration.Factory.CreateObject("tracking/contactManager", true) as ContactManager;
            }
        }


        public static string GetContactFacetsXmlTree()
        {
            XElement parent = new XElement((XName)"root");
            //ContactFacetHelper.CreateNode(parent, "facets", true, "");
            //parent.Add((object)new XElement((XName)"isFolder", (object)true));
            //parent.Add((object)new XElement((XName)"expand", (object)true));
            foreach (KeyValuePair<string, IFacet> keyValuePair in ContactFacetHelper.FacetFactory.ContactFacets)
            {
                XElement node = new XElement((XName)keyValuePair.Key);
                parent.Add((object)node);
                ContactFacetHelper.ExpandElement(node, (IElement)keyValuePair.Value, keyValuePair.Key, "");
            }
            return DependenciesManager.ConvertionUtil.ConvertToJson((object)parent);
        }

        private static void CreateNode(XElement parent, string title, bool folder = true, string key = "")
        {
            Assert.ArgumentNotNullOrEmpty(title, "title");
            parent.Add((object)new XElement((XName)"title", (object)title));
            //if (folder)
            //{
            //    parent.Add((object)new XElement((XName)"isFolder", (object)true));
            //    parent.Add((object)new XElement((XName)"expand", (object)true));
            //}
            if (string.IsNullOrEmpty(key.Trim()))
                return;
            parent.Add((object)new XElement((XName)"key", (object)key.Trim()));
        }

        private static void ExpandElement(XElement node, IElement element, string title, string key = "")
        {
            Assert.ArgumentNotNull((object)node, "node");
            Assert.ArgumentNotNull((object)element, "element");
            if (string.IsNullOrEmpty(key.Trim()))
                key = title;
            IEnumerable<IModelMember> facetMembers = ContactFacetHelper.FacetFactory.GetFacetMembers(element);
            if (facetMembers == null)
                return;
            if (!string.IsNullOrEmpty(title))
                ContactFacetHelper.CreateNode(node, title, true, key);
            foreach (IModelMember modelMember in facetMembers)
            {
                if (modelMember is IModelAttributeMember && !string.Equals(modelMember.Name, "Preferred", StringComparison.OrdinalIgnoreCase))
                {
                    XElement parent = new XElement((XName)"children");
                    node.Add((object)parent);
                    ContactFacetHelper.CreateNode(parent, modelMember.Name, false, key + "/" + modelMember.Name);
                }
                else if (modelMember is IModelElementMember)
                {
                    IModelElementMember modelElementMember = modelMember as IModelElementMember;
                    XElement node1 = new XElement((XName)"children");
                    node.Add((object)node1);
                    ContactFacetHelper.ExpandElement(node1, modelElementMember.Element, modelElementMember.Name, key + "/" + modelElementMember.Name);
                }
                else if (modelMember is IModelDictionaryMember || modelMember is IModelCollectionMember)
                {
                    IModelDictionaryMember dictionaryMember = modelMember as IModelDictionaryMember;
                    Type type = dictionaryMember == null ? Enumerable.Single<Type>((IEnumerable<Type>)(modelMember as IModelCollectionMember).Elements.GetType().GetGenericArguments()) : Enumerable.Single<Type>((IEnumerable<Type>)dictionaryMember.Elements.GetType().GetGenericArguments());
                    if (!(type == (Type)null))
                        ContactFacetHelper.ExpandElement(node, ContactFacetHelper.FacetFactory.CreateElement(type), string.Empty, key + "/Entries");
                }
            }
        }
    }
}
