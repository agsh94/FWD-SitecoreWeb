using System.Collections.Generic;

namespace FWD.Foundation.CdnPurgeUtility.Models
{
    public class RestApiInputParameter
    {
        public List<string> MediaPath { get; set; }
        public int MediaType { get; set; }
        public RestApiInputParameter(List<string> mediaPath, int mediaType)
        {
            MediaPath = mediaPath;
            MediaType = mediaType;
        }
    }
}