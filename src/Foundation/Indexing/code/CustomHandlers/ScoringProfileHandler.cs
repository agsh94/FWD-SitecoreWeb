/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Indexing.Helpers;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.Diagnostics;
using Sitecore.Events;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FWD.Foundation.Indexing.CustomHandlers
{
    [ExcludeFromCodeCoverage]
    public class ScoringProfileHandler
    {
        public static ISearchServiceClient SearchServiceClient { get; set; }

        public void AddScoringProfile(object sender, EventArgs args)
        {
            try
            {
                string parameter1 = Event.ExtractParameter<string>(args, 0); // cmsIndexName
                string parameter3 = Event.ExtractParameter<string>(args, 2); // rebuildIndexName

                ISearchIndex searchIndex;

                if (!ContentSearchManager.SearchConfiguration.Indexes.TryGetValue(parameter1, out searchIndex) || !(searchIndex is CloudSearchProviderIndex))
                {
                    Log.Warn($"{CustomScoringProfile.Messages.CmsIndexNotFound} {parameter1}", this.GetType().Name);
                    return;
                }

                var database = Factory.GetDatabase(((SitecoreItemCrawler)searchIndex.Crawlers.FirstOrDefault()).Database);

                var spSettings = database?.GetItem(CustomScoringProfile.Fields.GlobalItemId);

                if (spSettings == null)
                {
                    Log.Warn($"{CustomScoringProfile.Messages.WeightSettingNotFound}", this.GetType().Name);
                    return;
                }

                string[] connArray = ConfigurationManager.ConnectionStrings[ConStr.Name.cldSearch].ConnectionString.Split(ConStr.pipeSeparator, StringSplitOptions.RemoveEmptyEntries);

                foreach (string conStr in connArray)
                {
                    var urlItems = GetServiceCredential(conStr);

                    if (string.IsNullOrEmpty(urlItems.Item1) || string.IsNullOrEmpty(urlItems.Item2))
                    {
                        Log.Warn($"{CustomScoringProfile.Messages.SearchServicdeKeyNameEmpty}", this.GetType().Name);
                        return;
                    }

                    using (SearchServiceClient = new SearchServiceClient(urlItems.Item1.Trim(), new SearchCredentials(urlItems.Item2.Trim())))
                    {
                        Index index = SearchServiceClient?.Indexes?.Get(parameter3);

                        SearchServiceClient.Indexes.CreateOrUpdate(SearchHelper.UpdateIndex(index, spSettings, database));

                        Log.Info($"{CustomScoringProfile.Messages.CreatedSuccessfully} {parameter3}", this.GetType().Name);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}", this.GetType().Name);
            }
        }

        private Tuple<string, string> GetServiceCredential(string conStr)
        {
            if (string.IsNullOrEmpty(conStr)) return null;

            var conStrProps = conStr.Split(ConStr.semiColonSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (conStrProps?.Any() == false) return null;

            var serviceUrl = conStrProps?.First(x => x.Contains(ConStr.Prop.svrUrlScheme) && x.Contains(ConStr.Prop.svrUrlPattern));

            if (serviceUrl?.Any() == false) return null;

            serviceUrl = !string.IsNullOrEmpty(serviceUrl) ? serviceUrl.Replace(ConStr.Prop.svrUrlScheme, string.Empty).Replace(ConStr.Prop.svrUrlPattern, string.Empty) : string.Empty;

            var apiKey = conStrProps?.First(x => x.Contains(ConStr.Prop.apiKeyPattern));

            if (apiKey?.Any() == false) return null;

            apiKey = !string.IsNullOrEmpty(apiKey) ? apiKey.Replace(ConStr.Prop.apiKeyPattern, string.Empty) : string.Empty;

            return Tuple.Create(serviceUrl, apiKey);
        }

    }
}