/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Dianoga.Optimizers;
using System;
using System.IO;

namespace FWD.Foundation.Dianoga.Pipelines.DianogaPng
{
    public class CustomPngOptimizer : CustomCommandLineToolOptimizer
    {
        protected override bool OptimizerUsesSeparateOutputFile
        {
            get
            {
                return false;
            }
        }

        protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
        {
            return "-file \"" + tempFilePath + "\"";
        }

        protected override string GetTempFilePath()
        {
            try
            {
                string tempFileName = Path.GetTempFileName();
                File.Delete(tempFileName);
                return Path.ChangeExtension(tempFileName, ".png");
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Error occurred while creating temp file to optimize. This can happen if IIS does not have write access to " + Path.GetTempPath() + ", or if the temp folder has 65535 files in it and is full.", (Exception)ex);
            }
        }
    }
}