/*9fbef606107a605d69c0edbcd8029e5d*/
using Dianoga.Optimizers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace FWD.Foundation.Dianoga.Optimizers
{
    public abstract class CustomCommandLineToolOptimizer : CommandLineToolOptimizer
    {
        protected override void ExecuteProcess(string arguments)
        {
            ConcurrentBag<string> processOutput = new ConcurrentBag<string>();
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = this.ExePath,
                Arguments = arguments
            };
            Sitecore.Diagnostics.Log.Info("\"" + this.ExePath + " " + arguments + "\"", (object)this);
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (DataReceivedEventHandler)((sender, eventArgs) => processOutput.Add(eventArgs.Data));
            process.ErrorDataReceived += (DataReceivedEventHandler)((sender, eventArgs) => processOutput.Add(eventArgs.Data));
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("\"" + this.ExePath + " " + arguments + "\" could not be started because an error occurred. See the inner exception for details.", ex);
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!process.WaitForExit(this.ToolTimeout))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }
                throw new InvalidOperationException(string.Format("\"{0} {1}\" took longer than {2}ms to run, which is a failure. Output: {3}", (object)this.ExePath, (object)arguments, (object)this.ToolTimeout, (object)string.Join(Environment.NewLine, (IEnumerable<string>)processOutput)));
            }
        }
    }
}