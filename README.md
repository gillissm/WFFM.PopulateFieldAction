# WFFM.PopulateFieldAction
Source code to support custom Sitecore Web Forms for Marketers (WFFM) actions, rule macro, and conditions allowing the content editor to leverage data collected in xProfiles.

These custom item requires a minimum version of Sitecore 8 update 6 OR Sitecore 8 update 1 to properly work, due to large shifts in WFFM libraries that are used.

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
