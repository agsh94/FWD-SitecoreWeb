/*9fbef606107a605d69c0edbcd8029e5d*/
using System;
using System.Collections.Generic;
using System.Text;
using FWD.Foundation.SitecoreExtensions.Pipelines;
using FWD.Foundation.Testing.Attributes;
using NSubstitute;
using Sitecore.Data;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using Xunit;

namespace FWD.Foundation.SitecoreExtensions.Tests.Pipelines
{
    public class CustomRenderingParametersResolverTests
    {
        [Theory]
        [AutoDbData]
        public void ProcessCustomRenderingParametersResolverTest()
        {
            IConfiguration configuration = Substitute.For<IConfiguration>();
            CustomRenderingParametersResolver customRenderingParametersResolver = new CustomRenderingParametersResolver(configuration);

            var testKeyItemID1 = ID.NewID;
            var testKeyItemID2 = ID.NewID;

            using (Sitecore.FakeDb.Db db = new Sitecore.FakeDb.Db
             {
                new Sitecore.FakeDb.DbItem("MasterDataMockItemId", testKeyItemID1)
                {
                    { "value", RandomString(6) } 
                },
                new Sitecore.FakeDb.DbItem("MasterDataMockItemId", testKeyItemID2)
                {
                    { "value", RandomString(8) }
                }
             })
            {
                RenderJsonRenderingArgs args = new RenderJsonRenderingArgs();

                args.RenderingConfiguration = Substitute.For<IRenderingConfiguration>();
                args.Result = new RenderedJsonRendering();
                args.Result.RenderingParams = new Dictionary<string, string>();

                args.Result.RenderingParams.Add("TestKey1", "TestValue1");

                args.Result.RenderingParams.Add("TestKey2", testKeyItemID1.ToString());

                args.Result.RenderingParams.Add("TestKey3", testKeyItemID2.ToString());

                args.Result.RenderingParams.Add("TestKey4", testKeyItemID2.ToString() + "|" + testKeyItemID1.ToString());

                customRenderingParametersResolver.Process(args);
            }

        
        }

    private string RandomString(int size)
    {
        StringBuilder builder = new StringBuilder();
        Random random = new Random();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(random.Next(65, 91));
            builder.Append(ch);
        }

        return builder.ToString();
    }
}
}