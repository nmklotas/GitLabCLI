using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommandLine;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using Microsoft.Extensions.Configuration;
using NGitLab;

namespace GitlabCmd.Console.App
{
    public static class Container
    {
        public static WindsorContainer Build()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<Parser>());
            container.Register(Component.For<GitLabFacade>());
            container.Register(Component.For<LaunchHandler>());
            container.Register(Component.For<ParametersHandler>());
            container.Register(Component.For<OutputPresenter>());
            container.Register(Component.For<GitLabIssueHandler>());
            container.Register(Component.For<MergeRequestsHandler>());
            container.Register(Component.For<AppSettingsValidationHandler>());

            container.Register(Component.For<Lazy<GitLabClientEx>>().
                UsingFactoryMethod(c => new Lazy<GitLabClientEx>(() =>
                {
                    var settings = c.Resolve<AppSettings>();
                    return new GitLabClientEx(settings.GitLabHostUrl, settings.GitLabAccessToken);
                })));

            RegisterAppSettings(container);
            return container;
        }

        private static void RegisterAppSettings(WindsorContainer container)
        {
            var builder = new ConfigurationBuilder().
                AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).
                AddEnvironmentVariables();

            container.Register(Component.For<AppSettings>().UsingFactoryMethod(c =>
            {
                var appSettings = new AppSettings();
                builder.Build().GetSection("appConfiguration").Bind(appSettings);
                return appSettings;
            }));
        }
    }
}