using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Output;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.GitLab;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GitLabCLI.Console
{
    public static class Container
    {
        public static WindsorContainer Build()
        {
            var container = new WindsorContainer();

            RegisterApplicationServices(container);
            RegisterGitLabServices(container);
            RegisterSettingsServices(container);

            return container;
        }

        private static void RegisterApplicationServices(WindsorContainer container) 
            => container.
                Register(Component.For<Parser>().UsingFactoryMethod(c => Parser.Default)).
                Register(Component.For<CommandLineArgsParser>()).
                Register(Component.For<LaunchHandler>()).
                Register(Component.For<ParametersHandler>()).
                Register(Component.For<OutputPresenter>()).
                Register(Component.For<RowResultFormatter>()).
                Register(Component.For<ConfigurationHandler>()).
                Register(Component.For<AppSettingsValidator>()).
                Register(Component.For<GridResultFormatter>()).
                Register(Component.For<LaunchOptionsVisitor>()).
                Register(Component.For<ConsoleColoredWriter>());

        private static void RegisterGitLabServices(WindsorContainer container) 
            => container.
                Register(Component.For<IGitLabFacade>().ImplementedBy<GitLabFacade>()).
                Register(Component.For<GitLabClientFactory>()).
                Register(Component.For<GitLabIssuesFacade>()).
                Register(Component.For<GitLabMergesFacade>()).
                Register(Component.For<GitLabIssueHandler>()).
                Register(Component.For<GitLabMergeRequestsHandler>()).
                Register(Component.For<Mapper>()).
                Register(Component.For<JsonSerializer>().Instance(CreateJsonSerializer()));

        private static void RegisterSettingsServices(WindsorContainer container) 
            => container.
                Register(Component.For<AppSettingsStorage>().UsingFactoryMethod(c => new AppSettingsStorage(c.Resolve<JsonSerializer>(), GetSettingsFile()))).
                Register(Component.For<AppSettings>().UsingFactoryMethod(c => c.Resolve<AppSettingsStorage>().Load())).
                Register(Component.For<GitLabSettings>().UsingFactoryMethod(c => Map(c.Resolve<AppSettings>())));

        private static string GetSettingsFile()
        {
            string localAppDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(localAppDataFolder, "gitlab", "appsettings.json");
        }

        private static JsonSerializer CreateJsonSerializer() 
            => JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        private static GitLabSettings Map(AppSettings settings)
        {
            return new GitLabSettings
            {
                GitLabAccessToken = settings.GitLabAccessToken,
                GitLabHostUrl = settings.GitLabHostUrl,
                GitLabPassword = settings.GitLabPassword,
                GitLabUserName = settings.GitLabUserName
            };
        }
    }
}