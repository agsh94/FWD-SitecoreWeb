/*9fbef606107a605d69c0edbcd8029e5d*/
using FWD.Foundation.Dianoga.Optimizers;

namespace FWD.Foundation.Dianoga.Pipelines.DianogaJpeg
{
    public class CustomMozJpegOptimizer : CustomCommandLineToolOptimizer
    {
        protected override string CreateToolArguments(string tempFilePath, string tempOutputPath)
        {
            return "-optimise -copy none -outfile \"" + tempOutputPath + "\" \"" + tempFilePath + "\"";
        }
    }
}