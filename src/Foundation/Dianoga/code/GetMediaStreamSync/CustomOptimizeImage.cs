/*9fbef606107a605d69c0edbcd8029e5d*/
using Dianoga;
using Dianoga.Invokers.GetMediaStreamSync;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using System;

namespace FWD.Foundation.Dianoga.GetMediaStreamSync
{
    public class CustomOptimizeImage : OptimizeImage
    {
        private readonly MediaOptimizer _optimizer;

        public CustomOptimizeImage()
          : this(new MediaOptimizer())
        {
        }
        protected CustomOptimizeImage(MediaOptimizer optimizer)
        {
            Assert.ArgumentNotNull((object)optimizer, nameof(optimizer));
            this._optimizer = optimizer;
        }
        public new void Process(GetMediaStreamPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.Options.Thumbnail || Context.Site.Name == "shell")
                return;
            if (args.OutputStream.Extension.Equals(GlobalConstants.PdfExtension, StringComparison.OrdinalIgnoreCase))
                return;
            MediaStream outputStream = args.OutputStream;
            if (outputStream == null)
                return;
            if (!outputStream.AllowMemoryLoading)
            {
                Tracer.Error((object)"Could not resize image as it was larger than the maximum size allowed for memory processing. Media item: {0}", (object)outputStream.MediaItem.Path);
            }
            else
            {
                MediaStream mediaStream = this._optimizer?.Process(outputStream, args.Options);
                if (mediaStream != null && outputStream.Stream != mediaStream.Stream)
                {
                    outputStream.Dispose();
                    args.OutputStream = mediaStream;
                    if (!(mediaStream.Extension == "webp"))
                        return;
                    args.AbortPipeline();
                }
                else
                    Log.Info("Dianoga: " + outputStream.MediaItem.MediaPath + " cannot be optimized due to media type or path exclusion", (object)this);
            }
        }
    }
}