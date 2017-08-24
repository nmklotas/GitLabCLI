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
        {
            container.Register(Component.For<Parser>().UsingFactoryMethod(c => Parser.Default));
            container.Register(Component.For<CommandLineArgsParser>());
            container.Register(Component.For<LaunchHandler>());
            container.Register(Component.For<ParametersHandler>());
            container.Register(Component.For<OutputPresenter>());
            container.Register(Component.For<ConfigurationHandler>());
            container.Register(Component.For<AppSettingsValidator>());
            container.Register(Component.For<GridResultFormatter>());
            container.Register(Component.For<LaunchOptionsVisitor>());
        }

        private static void RegisterGitLabServices(WindsorContainer container)
        {
            container.Register(Component.For<IGitLabFacade>().ImplementedBy<GitLabFacade>());
            container.Register(Component.For<GitLabClientExFactory>());
            container.Register(Component.For<GitLabIssuesFacade>());
            container.Register(Component.For<GitLabMergesFacade>());
            container.Register(Component.For<GitLabIssueHandler>());
            container.Register(Component.For<GitLabMergeRequestsHandler>());
            container.Register(Component.For<Mapper>());

            container.Register(Component.For<JsonSerializer>().UsingFactoryMethod(c => JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));
        }

        private static void RegisterSettingsServices(WindsorContainer container)
        {
            container.Register(Component.For<AppSettingsStorage>().UsingFactoryMethod(
                c => new AppSettingsStorage(
                    c.Resolve<JsonSerializer>(),
                    GetSettingsFile())));

            container.Register(Component.For<AppSettings>().UsingFactoryMethod
                (c => c.Resolve<AppSettingsStorage>().Load()));

            container.Register(Component.For<GitLabSettings>().UsingFactoryMethod
                (c => Map(c.Resolve<AppSettings>())));
        }

        private static string GetSettingsFile()
        {
            string localAppDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(localAppDataFolder, "gitlab", "appsettings.json");
        }

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