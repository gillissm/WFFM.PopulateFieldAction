# WFFM.PopulateFieldAction
Source code to support custom Sitecore Web Forms for Marketers (WFFM) actions, rule macro, and conditions allowing the content editor to leverage data collected in xProfiles.

Be aware due to some WFFM class changes this module is limited to Sitecore 8.0 Update 6 and above or Sitecore 8.1 Update 1 and above and their corresponding WFFM installs.

This module adds three items to enhance the usage of Web Forms for Marketers (WFFM).
    1.There is a custom Rule Macro, that loads a picker list to select a contact facet field (xProfile property) for use in any rule or action.
    2.A new WFFM field action, that populates the value of the field from a selected contact facet via the Rule Macro.
    3.Finally, you get a new WFFM rule, which will hide a WFFM field if the selected contact facet field already contains a value, because it is silly to ask a user to fill in the same information multiple times.

As a bonus, there is a WFFM form that uses all three of the above for you to see how it works.

Project includes TDS support and Sitecore package for installation.

Sitecore package includes the following:
File system are:
* bin \ TheCodeAttic.SharedSource.WFFM.PopulateFieldAction.dll
* Sitecore modules \ shell \ TheCodeAttic \ ContactFacetDialog.xaml.xml

Sitecore Artifacts Include:
* content \ Web forms \ CoffeeForLifeEntry [sample form using the rules and actions]
* system \ Settings \ Rules \ Definitions \ Elements \ Visitor \ Is Contact Facet Member Populated [condition]
* system \ Settings \ Rules \ Definitions \ Elements \ Web Forms for Marketers \ Read Value from Contact Facet [action]
* system \ Settings \ Rules \ Definitions \ Macros \ ContactFacet
