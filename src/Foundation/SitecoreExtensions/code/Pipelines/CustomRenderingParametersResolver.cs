/*9fbef606107a605d69c0edbcd8029e5d*/
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;

namespace FWD.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomRenderingParametersResolver : BaseRenderJsonRendering
    {
        public CustomRenderingParametersResolver(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Overrides the Rendering Parameters in the JSS Layout Service.
        /// </summary>
        /// <param name="args"></param>
        protected override void SetResult(RenderJsonRenderingArgs args)
        {
            if (args?.RenderingConfiguration != null)
            {
                IDictionary<string, string> paramKeyValues = args.Result.RenderingParams;

                if (paramKeyValues != null && paramKeyValues.Count > 0)
                {
                    foreach (var param in args.Result.RenderingParams.ToList())
                    {
                        string value = string.Empty;

                        value = GetValueFromMultiListParameter(param.Value);

                        if (!string.IsNullOrEmpty(value))
                        {
                            paramKeyValues[param.Key] = value;
                        }
                        else
                        {
                            paramKeyValues[param.Key] = string.Empty;
                        }
                    }
                    args.Result.RenderingParams = paramKeyValues;
                }
            }
        }

        /// <summary>
        /// Returns the Pipe Seperated string value of the data passed in Rendering Parameter.
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private string GetValueFromMultiListParameter(string paramValue)
        {
            List<string> valuesList = new List<string>();

            var idList = paramValue.Split(CharacterConstants.PipeSymbol);
            foreach (var id in idList)
            {
                string value = GetValue(id);
                if (!string.IsNullOrEmpty(value))
                {
                    valuesList.Add(value);
                }
            }

            return string.Join(CharacterConstants.PipeSymbol.ToString(), valuesList);
        }

        /// <summary>
        /// Checks if input value is Valid Sitecore ID and return its corresponding value.
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private string GetValue(string paramValue)
        {
            string value = string.Empty;
            ID sourceId = null;

            if (ID.TryParse(paramValue, out sourceId))
            {
                var sourceItem = Sitecore.Context.Database.GetItem(sourceId);
                if (sourceItem != null)
                {
                    value = sourceItem[FieldNames.Value];
                }
            }
            else
            {
                value = paramValue;
            }

            return value;
        }
    }
}