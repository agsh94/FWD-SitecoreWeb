/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.API.Helper;
using Newtonsoft.Json.Linq;
using System.IO;
using Xunit;

namespace FWD.Foundation.API.Tests.Helper
{
    public class ApiHelperTest
    {
        [Fact]
        public void GetProductRequestDataTest()
        {
            var path = Directory.GetCurrentDirectory() + "\\Data\\ApiBaseData.json";
            var data = JObject.Parse(File.ReadAllText(path));

            var result = ApiHelper.GetApiBaseData(data, "en");

            Assert.NotNull(result.ApiEndPoint);
            Assert.NotNull(result.AuthToken);
            Assert.NotNull(result.HeaderData);

            string response = ApiHelper.GetProductRequestData(result.HeaderData, null, "saving", "cancer", "Product", "Online", true);

            Assert.NotNull(response);
        }
    }
}