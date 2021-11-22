/*9fbef606107a605d69c0edbcd8029e5d*/
using Sitecore.Buckets.Interfaces;
using Sitecore.Buckets.Util;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sitecore.Buckets.Search.Tags;
using Sitecore.Data.Items;
using Sitecore.Web;
using System.Web;

namespace FWD.Foundation.SitecoreExtensions.Tags
{
    public class SitecoreHostedListItemRepository : ITagRepository
    {
        /// <summary>The all.</summary>
        /// <returns>The System.Collections.Generic.IEnumerable`1[T -&gt; Sitecore.Buckets.Common.Providers.Tag].</returns>
        public IEnumerable<Tag> All()
        {
            SitecoreIndexableItem sitecoreIndexableItem = (SitecoreIndexableItem)(Sitecore.Context.ContentDatabase == null ? Sitecore.Context.Database.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID) : Sitecore.Context.ContentDatabase.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID));
            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)sitecoreIndexableItem).CreateSearchContext(SearchSecurityOptions.Default))
            {
                List<Tag> tagList = new List<Tag>();
                IQueryable<SitecoreUISearchResultItem> queryable = searchContext.GetQueryable<SitecoreUISearchResultItem>((IExecutionContext)new CultureExecutionContext(sitecoreIndexableItem.Culture));
                Expression<Func<SitecoreUISearchResultItem, bool>> predicate = (Expression<Func<SitecoreUISearchResultItem, bool>>)(ancestor => ancestor["_template"] == Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(BucketConfigurationSettings.TagTemplateId, true));
                foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)queryable.Where<SitecoreUISearchResultItem>(predicate))
                {
                    if (searchResultItem != null)
                        tagList.Add(new Tag(searchResultItem.GetItem().Name, searchResultItem.GetItem().ID.ToString()));
                }
                return (IEnumerable<Tag>)tagList;
            }
        }

        /// <summary>The first.</summary>
        /// <param name="exp">The exp.</param>
        /// <returns>The Sitecore.Buckets.Common.Providers.Tag.</returns>
        public Tag First(Func<Tag, bool> exp)
        {
            SitecoreIndexableItem sitecoreIndexableItem = (SitecoreIndexableItem)(Sitecore.Context.ContentDatabase == null ? Sitecore.Context.Database.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID) : Sitecore.Context.ContentDatabase.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID));
            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)sitecoreIndexableItem).CreateSearchContext(SearchSecurityOptions.Default))
            {
                IQueryable<SitecoreUISearchResultItem> queryable = searchContext.GetQueryable<SitecoreUISearchResultItem>((IExecutionContext)new CultureExecutionContext(sitecoreIndexableItem.Culture));
                Expression<Func<SitecoreUISearchResultItem, bool>> predicate = (Expression<Func<SitecoreUISearchResultItem, bool>>)(ancestor => ancestor["_template"] == Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(BucketConfigurationSettings.TagTemplateId, true));
                foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)queryable.Where<SitecoreUISearchResultItem>(predicate))
                {
                    if (searchResultItem != null)
                        return new Tag(searchResultItem.GetItem().Name, searchResultItem.GetItem().ID.ToString());
                }
                throw new InvalidOperationException();
            }
        }

        /// <summary>The get tag by value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The Sitecore.Buckets.Common.Providers.Tag.</returns>
        public Tag GetTagByValue(string value)
        {
            SitecoreIndexableItem sitecoreIndexableItem = (SitecoreIndexableItem)(Sitecore.Context.ContentDatabase == null ? Sitecore.Context.Database.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID) : Sitecore.Context.ContentDatabase.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID));
            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)sitecoreIndexableItem).CreateSearchContext(SearchSecurityOptions.Default))
            {
                
                IQueryable<SitecoreUISearchResultItem> queryable = searchContext.GetQueryable<SitecoreUISearchResultItem>((IExecutionContext)new CultureExecutionContext(sitecoreIndexableItem.Culture));
                Expression<Func<SitecoreUISearchResultItem, bool>> predicate = (Expression<Func<SitecoreUISearchResultItem, bool>>)(ancestor => ancestor["_group"] == Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(value, true));
                foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)queryable.Where<SitecoreUISearchResultItem>(predicate))
                {
                    if (searchResultItem != null)
                        return new Tag(searchResultItem.GetItem().Name, searchResultItem.GetItem().ID.ToString());
                }
                throw new InvalidOperationException();
            }
        }

        /// <summary>The get tags.</summary>
        /// <param name="contains">The contains.</param>
        /// <returns>The System.Collections.Generic.IEnumerable`1[T -&gt; Sitecore.Buckets.Common.Providers.Tag].</returns>
        public IEnumerable<Tag> GetTags(string contains)
        {
            
            Item currentItem = Sitecore.Context.ContentDatabase.GetItem(WebUtil.ExtractUrlParm("id", HttpContext.Current.Request.UrlReferrer.AbsoluteUri));
            string query = string.Format("./ancestor-or-self::*[@@templateid='{0}']//*[@@templateid='{1}']", CommonConstants.SiteTemplateId, CommonConstants.TagFolderTemplateId);

            SitecoreIndexableItem indexableItem = (SitecoreIndexableItem)currentItem.Axes.SelectSingleItem(query);

            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)indexableItem).CreateSearchContext(SearchSecurityOptions.Default))
            {
                List<Tag> tagList = new List<Tag>();
                string normalizeGuid = Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(CommonConstants.TagTemplateID, true);
                IQueryable<SitecoreUISearchResultItem> queryable = searchContext.GetQueryable<SitecoreUISearchResultItem>((IExecutionContext)new CultureExecutionContext(indexableItem.Culture));
                Expression<Func<SitecoreUISearchResultItem, bool>> predicate = (Expression<Func<SitecoreUISearchResultItem, bool>>)(ancestor => ancestor["_name"].StartsWith(contains) && ancestor["_template"] == normalizeGuid);
                foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)queryable.Where<SitecoreUISearchResultItem>(predicate))
                {
                    if (searchResultItem != null)
                        tagList.Add(new Tag(searchResultItem.GetItem().Name, searchResultItem.GetItem().ID.ToString()));
                }
                return (IEnumerable<Tag>)tagList;
            }
        }

        /// <summary>The single.</summary>
        /// <param name="exp">The exp.</param>
        /// <returns>The Sitecore.Buckets.Common.Providers.Tag.</returns>
        public Tag Single(Func<Tag, bool> exp)
        {
            SitecoreIndexableItem sitecoreIndexableItem = (SitecoreIndexableItem)(Sitecore.Context.ContentDatabase == null ? Sitecore.Context.Database.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID) : Sitecore.Context.ContentDatabase.GetItem(((ReferenceField)Sitecore.Buckets.Util.Constants.SettingsItem.Fields["Tag Parent"]).TargetItem.ID));
            
            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)sitecoreIndexableItem).CreateSearchContext(SearchSecurityOptions.Default))
            {
                IQueryable<SitecoreUISearchResultItem> queryable = searchContext.GetQueryable<SitecoreUISearchResultItem>((IExecutionContext)new CultureExecutionContext(sitecoreIndexableItem.Culture));
                Expression<Func<SitecoreUISearchResultItem, bool>> predicate = (Expression<Func<SitecoreUISearchResultItem, bool>>)(ancestor => ancestor["_template"] == Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(BucketConfigurationSettings.TagRepositoryId, true));
                foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)queryable.Where<SitecoreUISearchResultItem>(predicate))
                {
                    if (searchResultItem != null)
                        return new Tag(searchResultItem.GetItem().Name, searchResultItem.GetItem().ID.ToString());
                }
                throw new InvalidOperationException();
            }
        }

        public static SiteInfo GetSite(Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList().Where(x=>!string.IsNullOrEmpty(x.HostName));

            SiteInfo currentSiteinfo = null;
            var matchLength = 0;
            foreach (var siteInfo in siteInfoList)
            {
                if (item.Paths.FullPath.StartsWith(siteInfo.RootPath, StringComparison.OrdinalIgnoreCase) && siteInfo.RootPath.Length > matchLength)
                {
                    matchLength = siteInfo.RootPath.Length;
                    currentSiteinfo = siteInfo;
                }
            }

            return currentSiteinfo;
        }


    }
}