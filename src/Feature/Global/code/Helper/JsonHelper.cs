/*9fbef606107a605d69c0edbcd8029e5d*/
using Newtonsoft.Json.Linq;
using System;

namespace FWD.Features.Global.Helper
{
    public static class JsonHelper
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
    }
}