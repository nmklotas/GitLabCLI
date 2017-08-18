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

            if (!appSettingsValidator.Validate())
                return 1;

            return launcHandler.Launch(args).Result;
        }
    }
}