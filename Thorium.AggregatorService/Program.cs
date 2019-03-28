using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Thorium.Core.MicroServices;
using Thorium.Core.MicroServices.Abstractions;
using Thorium.Core.MicroServices.ProxyGenerator;

namespace Thorium.Aggregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var gen = new GeneratorCliExtender();
            
            var svcRunner = new ServiceRunner(new List<ICliExtender>
            {
                new GeneratorCliExtender()
            });
            
            svcRunner.Run(new AggregatorServiceStartup(), args);
        }

    }

    public class GeneratorCliExtender: ICliExtender
    {
        public void ExtendCli(CommandLineApplication app)
        {
            
                new GeneratorCommandLineApp().AddCommandToApplication(app);
        }
    }
}
