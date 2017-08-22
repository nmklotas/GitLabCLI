using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GitlabCmd.Console.App
{
    public static class Container
    {
        public static WindsorContainer Build()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<Parser>().UsingFactoryMethod(c => Parser.Default));
            container.Register(Component.For<GitLabFacade>());
            container.Register(Component.For<LaunchHandler>());
            container.Register(Component.For<ParametersHandler>());
            container.Register(Component.For<OutputPresenter>());
            container.Register(Component.For<ConfigurationHandler>());
            container.Register(Component.For<GitLabIssueHandler>());
            container.Register(Component.For<GitLabMergeRequestsHandler>());
            container.Register(Component.For<AppSettingsValidator>());
            container.Register(Component.For<GitLabClientFactory>());
            container.Register(Component.For<GridResultFormatter>());
            container.Register(Component.For<LaunchOptionsVisitor>());

            container.Register(Component.For<JsonSerializer>().UsingFactoryMethod(c => JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            container.Register(Component.For<AppSettingsStorage>().UsingFactoryMethod(
                c => new AppSettingsStorage(
                    c.Resolve<JsonSerializer>(),
                    GetSettingsFile())));

            container.Register(Component.For<AppSettings>().UsingFactoryMethod(
                c => c.Resolve<AppSettingsStorage>().Load()));

            return container;
        }

        private static string GetSettingsFile()
        {
            string localAppDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(localAppDataFolder, "gitlab", "appsettings.json");
        }
    }
}