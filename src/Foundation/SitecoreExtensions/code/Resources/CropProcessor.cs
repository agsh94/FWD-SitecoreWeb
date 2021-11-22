/*9fbef606107a605d69c0edbcd8029e5d*/
using ImageProcessor;
using ImageProcessor.Imaging;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FWD.Foundation.SitecoreExtensions.Resources
{
    [ExcludeFromCodeCoverage]
    public class CropProcessor
    {
        private static readonly string[] IMAGE_EXTENSIONS = new string[5]
        {
      "bmp",
      "jpeg",
      "jpg",
      "png",
      "gif"
        };

        public void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            MediaStream outputStream = args.OutputStream;
            if (outputStream == null || !((IEnumerable<string>)CropProcessor.IMAGE_EXTENSIONS).Any<string>((Func<string, bool>)(i => i.Equals(args.MediaData.Extension, StringComparison.InvariantCultureIgnoreCase))))
                return;
            string customOption1 = args.Options.CustomOptions["cx"];
            string customOption2 = args.Options.CustomOptions["cy"];
            string customOption3 = args.Options.CustomOptions["cw"];
            string customOption4 = args.Options.CustomOptions["ch"];
            float result1;
            float result2;
            int result3;
            int result4;
            if (!string.IsNullOrEmpty(customOption1) && !string.IsNullOrEmpty(customOption2) && (float.TryParse(customOption1, out result1) && float.TryParse(customOption2, out result2)) && (!string.IsNullOrEmpty(customOption3) && int.TryParse(customOption3, out result3) && !string.IsNullOrEmpty(customOption4)) && int.TryParse(customOption4, out result4))
            {
                Stream stream = Stream.Synchronized(this.GetCroppedImage(result3, result4, result1, result2, outputStream.MediaItem));
                args.OutputStream = new MediaStream(stream, args.MediaData.Extension, outputStream.MediaItem);
            }
            else
            {
                if (!args.Options.Thumbnail)
                    return;
                TransformationOptions transformationOptions = args.Options.GetTransformationOptions();
                MediaStream thumbnailStream = args.MediaData.GetThumbnailStream(transformationOptions);
                if (thumbnailStream != null)
                    args.OutputStream = thumbnailStream;
            }
        }

        private Stream GetCroppedImage(int width, int height, float cx, float cy, MediaItem mediaItem)
        {
            MemoryStream memoryStream = new MemoryStream();
            Stream stream = mediaItem.GetMediaStream();
            if (stream != null && stream.CanRead)
            {
                Image image = Image.FromStream(mediaItem.GetMediaStream());
                ImageFactory imageFactory = new ImageFactory(false);
                imageFactory.Load(image);
                float[] centerCoordinates = new float[2] { cy, cx };
                imageFactory.Resize(new ResizeLayer(new Size(width, height), ResizeMode.Crop, AnchorPosition.Center, true, centerCoordinates, new Size?(), (List<Size>)null, new Point?())).Save((Stream)memoryStream);
                return (Stream)memoryStream;
            }
            return stream;
        }
    }
}