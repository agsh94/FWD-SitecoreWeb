Foundation.Indexing
================

## About Indexing
 
This component provides the necessary functionality required for Site Search. Along with site search, 
this component also contains computed index fields. 

Package Details
----------------

Package name : FWD.Foundation.Indexing

1. Configure a new package source to refer FWD Nuget packages.
![Source](images/source.png)
2. Mention this URL : https://dev-appx.tools.publicis.sapient.com/artifactory/api/nuget/nuget-release
3. Browse and install the package in the project.
 

## How to use 
 
1) If Lucene is search engine then Add  **FWD.Foundation.Indexing** nuget package referance to project. Below files will get added to project once installed this project.

![File1](images/file1.PNG)
![File2](images/file2.PNG)

1) If Solr is search engine then Add  **FWD.Foundation.Indexing.Solr** nuget package referance to project. Below files will get added to project once installed this project.

![File1](images/solrpackage.PNG)

Use this URL : https://dev-appx.tools.publicis.sapient.com/artifactory/api/nuget/nuget-release for nuget packages

2) Create provider which is resposible for formatting the search results. Follow the below steps to implement provider
   We have two ways to implement provider.
   Approach 1:
     - Create class which implements "ProviderBase, ISearchResultFormatter, IQueryPredicateProvider"
	 - ISearchResultFormatter” interface has below properties/methods.
		1.	Content Type: This holds the value of Facet title in search page.
		2.	Supported Templates: Holds the IDs of templates of items which needs to be shown on search results page.
		3.	Format Result: Method responsible for formatting the search results. 
		4.	GetQueryPredicate: Prepares the query expression to search incoming query text in item field values.
		NOTE: For Solr, Mention solr generated field names.
		![Source](images/searchprovider.PNG)
  Approach 2:
	 - Create a class which inheriate from "BaseIndexingProvider" class and pass the Item id of Provider item to base class.
	 - Create Site specific Provider Item in Sitecore content tree and provide required information.
	  ![Source](images/provideritem.PNG)
	 - Create class which implements "ProviderBase, IQueryFacetProvider". This provider resposible for generating the Facets
	   dynamically in search results page based on "Content Type" parameter used in "ISearchResultFormatter" provider. This interface implments following 
	   method.
	    1. GetFacets: This returns the List of QueryFacets class which holds the title of "Facet Group" and value of facet field.
		![Source](images/facetprovider.PNG)
	 - Patch this newly created classes under <solutionFramework><indexing><providers> node

3) To use 'SearchService' of Indexing component, Create a class which implements 'IQuery' interface of Indexing component, which will take parameters 
	 - QueryText: Pass the entered the search query
	 - Page : start index for pagination
	 - NoOfResults : No of results to show on search result page.
	 

4) Create class which implements 'ISearchSettings' of indexing component.

5) Pass the object of class which implements 'ISearchSettings' to 'SearcService' class constructor. 
 
6) Call the search method of SearchSerice class by passing SearchQuery object, which will return search results.
![Source](images/indexing.PNG)

![File3](images/file3.PNG)
 
 ## Do's and Don't

 Ensure that only one search engine enabled for a website. We cannot enable multiple search engine for a given website.
 All the referances of Lucene should be removed from website when enabling Solr search engine.
 
 For Solr nuget package, we have enabled only below configurations. Other configurations can be enabled accrding to need basis.
	Sitecore.ContentSearch.Solr.Index.Core.config
	Sitecore.ContentSearch.Solr.Index.Master.config
	Sitecore.ContentSearch.Solr.Index.Web.config
	Sitecore.Speak.ContentSearch.Solr.config
	Sitecore.ContentSearch.Solr.DefaultIndexConfiguration.config
 

 ## Extension

 By default sitecore comes up with Lucene search engine. We can extend this component to other search engines like 'Solr', 'Coveo'
 When changing to other search engine. Some of config changes needs to be done for Indexing component. Below section in "Indexing" config will get changed.

			Lucene Config ->
						<fields hint="raw:AddComputedIndexField">
                            <field fieldName="has_presentation" storageType="no" indexType="untokenized">FWD.Foundation.Indexing.Infrastructure.Fields.HasPresentationComputedField, FWD.Foundation.Indexing</field>
                            <field fieldName="all_templates" storageType="no" indexType="untokenized">FWD.Foundation.Indexing.Infrastructure.Fields.AllTemplatesComputedField, FWD.Foundation.Indexing</field>
                            <field fieldName="has_search_result_formatter" storageType="no" indexType="untokenized">FWD.Foundation.Indexing.Infrastructure.Fields.HasSearchResultFormatterComputedField, FWD.Foundation.Indexing</field>
                            <field fieldName="search_result_formatter" storageType="no" indexType="untokenized">FWD.Foundation.Indexing.Infrastructure.Fields.SearchResultFormatterComputedField, FWD.Foundation.Indexing</field>
                        </fields>

			Solr Config ->
						<fields hint="raw:AddComputedIndexField">
							<field fieldName="has_presentation" returnType="string" >FWD.Foundation.Indexing.Infrastructure.Fields.HasPresentationComputedField, FWD.Foundation.Indexing</field>
							<field fieldName="all_templates" returnType="string" >FWD.Foundation.Indexing.Infrastructure.Fields.AllTemplatesComputedField, FWD.Foundation.Indexing</field>
							<field fieldName="has_search_result_formatter" returnType="string" >FWD.Foundation.Indexing.Infrastructure.Fields.HasSearchResultFormatterComputedField, FWD.Foundation.Indexing</field>
							<field fieldName="search_result_formatter" returnType="string" >FWD.Foundation.Indexing.Infrastructure.Fields.SearchResultFormatterComputedField, FWD.Foundation.Indexing</field>
						</fields>

  - Follow the below link to integrate Solr
  https://doc.sitecore.net/sitecore_experience_platform/setting_up_and_maintaining/search_and_indexing/walkthrough_setting_up_solr

  - Follow the below link to integrate Coveo
  https://developers.coveo.com/display/public/SitecoreV3/Integrating+Coveo+for+Sitecore+in+a+Website;jsessionid=9D35DF4D632695090B27CAB274D517D4