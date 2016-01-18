//STEP 1: Using Statements
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Diagnostics;
using Sitecore.Forms.Core.Rules;
using System.Linq;

//STEP 2: Namespace, Class, and Inheritance
namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class GetContactFacetValue<T> : ReadValue<T> where T : ConditionalRuleContext
    {
        protected override object GetValue()
        {
            //STEP 3: Protect ourselves, see Brian Perderson's explaination on Sitecore Asserts - https://briancaos.wordpress.com/2012/01/20/sitecore-diagnostics-assert-statements/
            Assert.IsNotNull((object)Tracker.Current, "Tracker.Current");
            Assert.IsNotNull((object)Tracker.Current.Contact, "Tracker.Current.Contact");

            string retValue = string.Empty;

            //STEP 4: Retreive from current contact's facets            
            switch (this.Name.ToLower())
            {
                case "first name":
                    retValue = Tracker.Current.Contact.GetFacet<IContactPersonalInfo>("Personal").FirstName;
                    break;
                case "last name":
                    retValue = Tracker.Current.Contact.GetFacet<IContactPersonalInfo>("Personal").Surname;
                    break;
                case "email":
                    IContactEmailAddresses ea = Tracker.Current.Contact.GetFacet<IContactEmailAddresses>("Emails");
                    if (!string.IsNullOrWhiteSpace(ea.Preferred) && ea.Entries.Contains(ea.Preferred))
                    {
                        retValue = ea.Entries[ea.Preferred].SmtpAddress;
                    }
                    else if (ea.Entries.Keys.Any())
                    {
                        retValue = ea.Entries[ea.Entries.Keys.First()].SmtpAddress;
                    }
                    break;
                case "city":
                    IContactAddresses ca = Tracker.Current.Contact.GetFacet<IContactAddresses>("Addresses");
                    if (!string.IsNullOrWhiteSpace(ca.Preferred) && ca.Entries.Contains(ca.Preferred))
                    {
                        retValue = ca.Entries[ca.Preferred].City;
                    }
                    else if (ca.Entries.Keys.Any())
                    {
                        retValue = ca.Entries[ca.Entries.Keys.First()].City;
                    }
                    break;
                case "phone":
                    IContactPhoneNumbers pn = Tracker.Current.Contact.GetFacet<IContactPhoneNumbers>("Phone Numbers");
                    if (!string.IsNullOrWhiteSpace(pn.Preferred) && pn.Entries.Contains(pn.Preferred))
                    {
                        retValue = string.Format("{0}-{1}", pn.Entries[pn.Preferred].CountryCode, pn.Entries[pn.Preferred].Number);
                    }
                    else if (pn.Entries.Keys.Any())
                    {
                        string firstKey = pn.Entries.Keys.First();
                        retValue = string.Format("{0}-{1}", pn.Entries[firstKey].CountryCode, pn.Entries[firstKey].Number);
                    }
                    break;
                default:
                    break;
            }

            return (object)retValue;
        }
    }
}
