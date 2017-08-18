using GitlabCmd.Console.App;

namespace GitlabCmd.Console
{
    public sealed class Program
    {
        public static int Main(string[] args)
        {
            var container = Container.Build();
            var appSettingsValidator = container.Resolve<AppSettingsValidationHandler>();
            var launcHandler = container.Resolve<LaunchHandler>();

            return appSettingsValidator.Validate() ? 
                launcHandler.Launch(args).Result : 
                ExitCode.InvalidConfiguration;
        }
    }
}