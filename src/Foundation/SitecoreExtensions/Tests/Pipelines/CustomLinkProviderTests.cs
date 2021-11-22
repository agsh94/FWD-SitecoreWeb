/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using FluentAssertions;
using FWD.Foundation.SitecoreExtensions.Pipelines;
using FWD.Foundation.Testing.Attributes;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Sites;
using Sitecore.Links;
using Xunit;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Tests.Pipelines
{
    public class CustomLinkProviderTests
    {
        [Theory]
        [AutoDbData]
        public void ProcessCustomLinkProviderTestWithOutLanguageCodeshouldReturnUrl(Db db)
        {
            var item = new DbItem("home", ID.NewID, ID.NewID);
            db?.Add(item);
            CustomLinkProvider provider = new CustomLinkProvider();

            var homeID = new ID();
            var siteConfigurationTempID = new ID("{485A5352-FF25-458A-8B3D-FD637DDB8ADC}");
            var siteConfigurationLinkTempID = new ID();
            var hidePrimaryLanguageTempID = new ID("{D9128379-7AC5-42D8-A12F-B1D34B2E2AAD}");

            var siteConfigurationLinkTargetItem = new DbItem("SiteConfigurationLinkTargetItem", siteConfigurationLinkTempID)
                                                            {
                                                                 new DbLinkField("hidePrimaryLanguage", hidePrimaryLanguageTempID)
                                                                    {
                                                                        Value ="1"
                                                                    }

                                                            };
            db?.Add(siteConfigurationLinkTargetItem);

            var homeItem = new DbItem("/sitecore/content/fwd/fwd-th", homeID)
                                                            {
                                                                 new DbLinkField("SiteConfigurationLink", siteConfigurationTempID)
                                                                   {
                                                                       Target = siteConfigurationLinkTempID.ToString(),
                                                                       TargetID = siteConfigurationLinkTempID,
                                                                       LinkType = "internal"
                                                                   }
                                                            };
            db?.Add(homeItem);

            var fakeSite = new FakeSiteContext(
                new Sitecore.Collections.StringDictionary
                  {
                    { "name", "website" },
                    { "database", db.Database.Name },
                    { "rootPath", homeID.ToString()},
                    { "startItem", "Home"},
                    { "language", "th"}
                });

            var defaultOptions = UrlOptions.DefaultOptions;
            defaultOptions.Site = fakeSite;
            var url = provider.GetItemUrl(db?.GetItem(item.ID), defaultOptions);
            url.Should().NotBeEmpty();

        }
    }
}