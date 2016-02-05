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

        private string _contactFacetElement = string.Empty;
        public string ContactFacetElement
        {
            get { return this._contactFacetElement; }
            set { this._contactFacetElement = value ?? string.Empty; }
        }

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


            //STEP 4: NEW

            retValue = GetFacet(this.Name, Tracker.Current.Contact);

            //STEP 4: Retreive from current contact's facets            
            //switch (this.Name.ToLower())
            //{
            //    case "first name":
            //        retValue = Tracker.Current.Contact.GetFacet<IContactPersonalInfo>("Personal").FirstName;
            //        break;
            //    case "last name":
            //        retValue = Tracker.Current.Contact.GetFacet<IContactPersonalInfo>("Personal").Surname;
            //        break;
            //    case "email":
            //        IContactEmailAddresses ea = Tracker.Current.Contact.GetFacet<IContactEmailAddresses>("Emails");
            //        if (!string.IsNullOrWhiteSpace(ea.Preferred) && ea.Entries.Contains(ea.Preferred))
            //        {
            //            retValue = ea.Entries[ea.Preferred].SmtpAddress;
            //        }
            //        else if (ea.Entries.Keys.Any())
            //        {
            //            retValue = ea.Entries[ea.Entries.Keys.First()].SmtpAddress;
            //        }
            //        break;
            //    case "city":
            //        IContactAddresses ca = Tracker.Current.Contact.GetFacet<IContactAddresses>("Addresses");
            //        if (!string.IsNullOrWhiteSpace(ca.Preferred) && ca.Entries.Contains(ca.Preferred))
            //        {
            //            retValue = ca.Entries[ca.Preferred].City;
            //        }
            //        else if (ca.Entries.Keys.Any())
            //        {
            //            retValue = ca.Entries[ca.Entries.Keys.First()].City;
            //        }
            //        break;
            //    case "phone":
            //        IContactPhoneNumbers pn = Tracker.Current.Contact.GetFacet<IContactPhoneNumbers>("Phone Numbers");
            //        if (!string.IsNullOrWhiteSpace(pn.Preferred) && pn.Entries.Contains(pn.Preferred))
            //        {
            //            retValue = string.Format("{0}-{1}", pn.Entries[pn.Preferred].CountryCode, pn.Entries[pn.Preferred].Number);
            //        }
            //        else if (pn.Entries.Keys.Any())
            //        {
            //            string firstKey = pn.Entries.Keys.First();
            //            retValue = string.Format("{0}-{1}", pn.Entries[firstKey].CountryCode, pn.Entries[firstKey].Number);
            //        }
            //        break;
            //    default:
            //        break;
            //}

            return (object)retValue;
        }


      
    }
}
