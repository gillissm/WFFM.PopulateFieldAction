//STEP 1: Using Statements
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Framework;
using Sitecore.Analytics.Tracking;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Data.Helpers;
using Sitecore.Forms.Core.Rules;
using Sitecore.WFFM.Abstractions.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//STEP 2: Namespace, Class, and Inheritance
namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class GetContactFacetValue<T> : ReadValue<T> where T : ConditionalRuleContext
    {        
        private IContactFacetFactory _contactFacetFactory;
        protected IContactFacetFactory contactFacetFactory {
            get{
                if(_contactFacetFactory==null)
                    _contactFacetFactory = ContactFacetsHelper.FacetFactory;

                return _contactFacetFactory;
            }
        }

        protected string GetFacet(string facetXpath, Contact contact)
        {
            string retValue = string.Empty;
            string[]xPath= facetXpath.Split('/');
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
                    PropertyInfo pi = element.GetType().GetProperty(memberName,BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    retValue = (string)pi.GetValue((object)element);
                }
                
            } 
            else if (modelMember is IModelAttributeMember && ((IModelAttributeMember)modelMember).Value != null)
            {
                PropertyInfo pi = facet.GetType().GetProperty(memberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                retValue = System.Convert.ChangeType((object)((IModelAttributeMember)modelMember).Value, pi.PropertyType).ToString();
            }
            else if (facetXpath.Remove(0, index.Length + 1).Length > 0)
                retValue= this.GetFacet(facetXpath.Remove(0, index.Length + 1), contact);
                        

            return retValue;
        }
        


        protected override object GetValue()
        {
            //STEP 3: Protect ourselves, see Brian Perderson's explaination on Sitecore Asserts - https://briancaos.wordpress.com/2012/01/20/sitecore-diagnostics-assert-statements/
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current");
            Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact");

            string retValue = string.Empty;


            //STEP 4: Retreival Based on Selected Member

            retValue = GetFacet(this.Name, Tracker.Current.Contact);
            return (object)retValue;
        }


      
    }
}
