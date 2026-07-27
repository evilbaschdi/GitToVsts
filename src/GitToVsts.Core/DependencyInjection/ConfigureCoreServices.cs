using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EvilBaschdi.Core.Settings.ByMachineAndUser;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.TeamServices;

namespace GitToVsts.Core.DependencyInjection;

/// <summary />
public static class ConfigureCoreServices
{
    /// <summary />
    public static void AddCoreServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IAppSettingsFromJsonFile, AppSettingsFromJsonFile>();
        services.AddSingleton<IAppSettingsFromJsonFileByMachineAndUser, AppSettingsFromJsonFileByMachineAndUser>();
        services.AddSingleton<IAppSettingByKey, AppSettingByKey>();
        services.AddSingleton<IApplicationSettings, ApplicationSettings>();

        services.AddTransient<IGitAvatar, ConvertGitAvatar>();
        services.AddTransient<IGitProcess, GetGitProcess>();
        services.AddTransient<IGitProcessInfo, GetGitProcessInfo>();
        services.AddTransient<IGitRepositories, GetGitRepositories>();
        services.AddTransient<IGitUser, GetGitUser>();
        services.AddTransient<IGitCommands, GitCommands>();

        services.AddTransient<ICreateProject, CreateProject>();
        services.AddTransient<ICreateRepository, CreateRepository>();
        services.AddTransient<IProjects, GetProjects>();
        services.AddTransient<IRepositories, GetRepositories>();
        services.AddTransient<ITemplates, GetTemplates>();
    }
}
