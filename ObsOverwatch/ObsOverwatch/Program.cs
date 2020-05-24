using System;
using Topshelf;

namespace ObsOverwatch
{
    public class Program
    {
        static void Main()
        {
            TopshelfExitCode topshelfExitCode = HostFactory.Run(configure =>
            {
                configure.Service<ObsOverwatchService>();
                configure.RunAsLocalSystem();
                configure.SetServiceName("OBS Overwatch");
                configure.SetDisplayName("OBS Overwatch");
                configure.SetDescription("Service that controls OBS program and manages automatic recovery from failure.");
            });

            int exitCode = (int)Convert.ChangeType(topshelfExitCode, topshelfExitCode.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
