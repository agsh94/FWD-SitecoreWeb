/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq.Expressions;
using FWD.Foundation.Indexing.Infrastructure;
using FWD.Foundation.Indexing.Models;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

#endregion

namespace FWD.Foundation.Multisite.Infrastructure.Indexing
{
    ///TODO: Migration Sitecore 9.0 - Update 1 
    ///Commented

    /// <summary>
    /// Local datasource query predicate provider
    /// </summary>
    public class LocalDatasourceQueryPredicateProvider : ProviderBase, IQueryPredicateProvider
    {
        public IEnumerable<ID> SupportedTemplates => new[]
      {
      TemplateIDs.StandardTemplate
    };

        public Expression<Func<SearchResultItem, bool>> GetQueryPredicate(IQuery query)
        {
            var fieldNames = new[]
            {
        Templates.Index.Fields.LocalDatasourceContentIndexFieldName
      };
            return GetFreeTextPredicateService.GetFreeTextPredicate(fieldNames, query);
        }
    }
}