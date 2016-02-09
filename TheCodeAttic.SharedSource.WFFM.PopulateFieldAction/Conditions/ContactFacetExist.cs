using Sitecore.Analytics;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Data.Helpers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.WFFM.Abstractions.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class ContactFacetExist<T>:OperatorCondition<T> where T:RuleContext
    {
        public string ContactFacetMemberPath { get; set; }

        private IContactFacetFactory _contactFacetFactory;
        protected IContactFacetFactory contactFacetFactory
        {
            get
            {
                if (_contactFacetFactory == null)
                    _contactFacetFactory = ContactFacetsHelper.FacetFactory;

                return _contactFacetFactory;
            }
        }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current must be not null");
            Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact must be not null");

            if (string.IsNullOrWhiteSpace(ContactFacetMemberPath) || Tracker.Current.Contact == null)
                return false;

            //Custom method that reads the facet member XML path and using reflection retrieves the value.
            var memberValue = GetFacet(ContactFacetMemberPath, Tracker.Current.Contact);            
            return !string.IsNullOrWhiteSpace(memberValue);
        }


        protected string GetFacet(string facetXpath, Contact contact)
        {
            string retValue = string.Empty;
            string[] xPath = facetXpath.Split('/');
            string index = xPath[0];
            string memberName = xPath[1];
            IFacet facet = contact.Facets[index];
            IEnumerable<IModelMember> members = contactFacetFactory.GetFacetMembers((IElement)facet);
            IModelMember modelMember = Enumerable.FirstOrDefault<IModelMember>(members, (Func<IModelMember, bool>)(x => x.Name == memberName));


            if (string.Equals(memberName, "Entries", StringComparison.OrdinalIgnoreCase))
            {
                IElementDictionary<IElement> elementDictionary = facet.GetType().GetProperty("Entries", BindingFlags.Instance | BindingFlags.Public).GetValue((object)facet) as IElementDictionary<IElement>;
                Assert.IsNotNull((object)elementDictionary, "Can't get facet entries.");
                IElement element;

                PropertyInfo property = facet.GetType().GetProperty("Preferred", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var preferredKey = (string)property.GetValue((object)facet);

                if (!string.IsNullOrWhiteSpace(preferredKey) &&
                    Enumerable.FirstOrDefault<string>((IEnumerable<string>)elementDictionary.Keys, (Func<string, bool>)(x => string.Equals(x, preferredKey, StringComparison.InvariantCultureIgnoreCase))) != null)
                {
                    element = elementDictionary[preferredKey];
                    memberName = xPath[2];
                    PropertyInfo pi = element.GetType().GetProperty(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    retValue = (string)pi.GetValue((object)element);
                }

            }
            else if (modelMember is IModelAttributeMember && ((IModelAttributeMember)modelMember).Value != null)
            {
                PropertyInfo pi = facet.GetType().GetProperty(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                retValue = System.Convert.ChangeType((object)((IModelAttributeMember)modelMember).Value, pi.PropertyType).ToString();
            }
            else if (facetXpath.Remove(0, index.Length + 1).Length > 0)
                retValue = this.GetFacet(facetXpath.Remove(0, index.Length + 1), contact);

            return retValue;
        }
    }
}