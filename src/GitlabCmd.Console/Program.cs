using System.Threading.Tasks;
using GitlabCmd.Console.App;

namespace GitlabCmd.Console
{
    public sealed class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var container = Container.Build();
            var appSettingsValidator = container.Resolve<AppSettingsValidationHandler>();
            var launcHandler = container.Resolve<LaunchHandler>();
            
            return appSettingsValidator.Validate() ? 
                await launcHandler.Launch(args) : 
                ExitCode.InvalidConfiguration;
        }
    }
}