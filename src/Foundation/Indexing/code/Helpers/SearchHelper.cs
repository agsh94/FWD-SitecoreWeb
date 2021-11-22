/*9fbef606107a605d69c0edbcd8029e5d*/
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Scfield = Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Managers;
using System.Diagnostics.CodeAnalysis;

namespace FWD.Foundation.Indexing.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class SearchHelper
    {
        public static Database GetIndexedDb(string indexName)
        {
            var searchIndex = ContentSearchManager.GetIndex(indexName);

            if (searchIndex == null || searchIndex.Crawlers == null || !(searchIndex.Crawlers.Any())) return null;

            string dbName = ((SitecoreItemCrawler)searchIndex.Crawlers.FirstOrDefault()).Database;

            if (string.IsNullOrEmpty(dbName)) return null;

            return Factory.GetDatabase(dbName);
        }

        public static string GetActiveIndexName(ISearchServiceClient searchServiceClient, string indexName)
        {
            //Azure maintains index name using Hyphen
            indexName = indexName.Replace("_", "-");

            return searchServiceClient?.Indexes?.ListNames()?.Where(x => x.Contains(indexName))?.LastOrDefault();
        }

        public static Index UpdateIndex(Index index, Item spSettings, Database database)
        {
            if (index == null) return index;

            Dictionary<string, Field> indexFields = index?.Fields?.ToDictionary(f => f.Name, StringComparer.InvariantCultureIgnoreCase);

            if (indexFields == null || !(indexFields.Any())) return index;

            if (index.ScoringProfiles == null)
            {
                index.ScoringProfiles = new List<ScoringProfile>();
            }

            Dictionary<string, double> cmsFields = null;

            //Free Text Scoring Profile
            cmsFields = GetNameLookUpList(spSettings, CustomScoringProfile.Fields.FreeText, database);
            if (cmsFields != null && cmsFields.Any())
            {
                var scoringProfile = GetScoringProfile(CustomScoringProfile.Name.FreeTextSearch, index, indexFields, cmsFields);

                if (scoringProfile.TextWeights.Weights.Count > 0)
                {
                    index.ScoringProfiles.Add(scoringProfile);
                }
            }

            return index;
        }

        private static Dictionary<string, double> GetNameLookUpList(Item spSettings, string spField, Database database)
        {
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();

            //Get the context languages
            var contextLanguages = GetContextLanguages(database);

            //Get Scoring Profile Settings as NameValueCollection
            var spItem = spSettings?.Fields[new ID(spField)];
            var nameValueCollection = Sitecore.Web.WebUtil.ParseUrlParameters(spItem?.Value);

            foreach (string key in nameValueCollection?.Keys)
            {
                Item lookupItem = database?.GetItem(MainUtil.GetID(key));

                foreach (var lang in contextLanguages)
                {
                    string fieldKey = ((Scfield.CheckboxField)lookupItem?.Fields[CustomScoringProfile.Fields.EnableAnalyzer]).Checked ?
                    GetLanguageContextField(lang, lookupItem?[CustomScoringProfile.Fields.Key]) : lookupItem?[CustomScoringProfile.Fields.Key];

                    double fieldValue = System.Convert.ToDouble(nameValueCollection?.GetValues(key).FirstOrDefault());

                    if (!keyValuePairs.ContainsKey(fieldKey))
                    {
                        keyValuePairs.Add(fieldKey, fieldValue);
                    }
                }
            }

            return keyValuePairs;
        }

        private static ScoringProfile GetScoringProfile(string scoringProfileName, Index index, Dictionary<string, Field> indexFields, Dictionary<string, double> cmsFields)
        {
            ScoringProfile scoringProfile = index?.ScoringProfiles?.FirstOrDefault(sp => sp.Name == scoringProfileName);

            if (scoringProfile == null)
            {
                scoringProfile = CreateScoringProfile(scoringProfileName);
                cmsFields.Where(x => !string.IsNullOrEmpty(x.Key))?.ToList()?.ForEach(x => AddFieldWeight(indexFields, scoringProfile, x));
            }
            return scoringProfile;
        }

        private static void AddFieldWeight(Dictionary<string, Field> indexFields, ScoringProfile scoringProfile, KeyValuePair<string, double> keyValuePair)
        {
            if (indexFields.ContainsKey(keyValuePair.Key) && !scoringProfile.TextWeights.Weights.ContainsKey(keyValuePair.Key))
            {
                scoringProfile.TextWeights.Weights.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        private static ScoringProfile GetScoringFunction(string scoringProfileName, Index index, Dictionary<string, Field> indexFields, Dictionary<string, double> cmsFields)
        {
            ScoringProfile scoringProfile = index?.ScoringProfiles?.FirstOrDefault(sp => sp.Name == scoringProfileName);

            if (scoringProfile == null)
            {
                scoringProfile = CreateScoringProfile(scoringProfileName);
                cmsFields.Where(x => !string.IsNullOrEmpty(x.Key) && !x.Key.ToLower().Equals(CustomScoringProfile.IndexedFields.PublishedDate))?.ToList()?.ForEach(x => AddFieldWeight(indexFields, scoringProfile, x));
            }

            if (cmsFields.ContainsKey(CustomScoringProfile.IndexedFields.PublishedDate) && indexFields.ContainsKey(CustomScoringProfile.IndexedFields.PublishedDate))
            {
                scoringProfile.Functions.Add(new FreshnessScoringFunction
                {
                    FieldName = CustomScoringProfile.IndexedFields.PublishedDate,
                    Boost = cmsFields.FirstOrDefault(x => x.Key.ToLower().Equals(CustomScoringProfile.IndexedFields.PublishedDate)).Value,
                    Parameters = new FreshnessScoringParameters(new TimeSpan(365, 0, 0, 0)),
                    Interpolation = ScoringFunctionInterpolation.Logarithmic
                });
            }

            return scoringProfile;
        }

        private static ScoringProfile CreateScoringProfile(string scoringProfileName)
        {
            switch (scoringProfileName)
            {
                case CustomScoringProfile.Name.FreeTextSearch:             
                    return GetWeightScoringObject(scoringProfileName);             
                default:
                    break;
            }
            return null;
        }

        private static ScoringProfile GetWeightScoringObject(string scoringProfileName)
        {
            return new ScoringProfile()
            {
                Name = scoringProfileName,
                TextWeights = new TextWeights(new Dictionary<string, double>())
            };
        }

        private static ScoringProfile GetFreshnessScoringObject(string scoringProfileName)
        {
            return new ScoringProfile()
            {
                Name = scoringProfileName,
                TextWeights = new TextWeights(new Dictionary<string, double>()),
                Functions = new List<ScoringFunction>() { }
            };
        }

        private static List<string> GetContextLanguages(Database database)
        {
            return LanguageManager.GetLanguages(database).Select(x => x.Name).ToList();
        }

        private static string GetLanguageContextField(string language, string fieldName)
        {
            return $"{fieldName}_t_{language}";
        }
    }
}