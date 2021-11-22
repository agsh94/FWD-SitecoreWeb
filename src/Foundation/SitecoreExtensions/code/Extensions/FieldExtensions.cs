/*9fbef606107a605d69c0edbcd8029e5d*/
#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Resources.Media;

#endregion

namespace FWD.Foundation.SitecoreExtensions.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class FieldExtensions
    {
        public static string ImageLink(this ImageField imageField)
        {
            if (imageField?.MediaItem == null)
                throw new ArgumentNullException(nameof(imageField));

            var options = MediaUrlOptions.Empty;
            int width, height;

            if (int.TryParse(imageField.Width, NumberStyles.Any, CultureInfo.InvariantCulture, out width))
                options.Width = width;

            if (int.TryParse(imageField?.Height, NumberStyles.Any, CultureInfo.InvariantCulture, out height))
                options.Height = height;
            return imageField.ImageLink(options);
        }

        public static string ImageLink(this ImageField imageField, MediaUrlOptions options)
        {
            if (imageField?.MediaItem == null)
                throw new ArgumentNullException(nameof(imageField));

            return options == null ? imageField?.ImageLink() : HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(imageField?.MediaItem, options));
        }

        public static bool IsChecked(this Field checkBoxField)
        {
            if (checkBoxField == null)
                throw new ArgumentNullException(nameof(checkBoxField));
            return MainUtil.GetBool(checkBoxField.Value, false);
        }
    }
}