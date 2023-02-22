﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusMods.CLI.OptionParsers;
using NexusMods.CLI.Verbs;
using NexusMods.Common;
using NexusMods.DataModel;
using NexusMods.DataModel.Games;
using NexusMods.DataModel.Interprocess;
using NexusMods.DataModel.Loadouts;
using NexusMods.DataModel.Loadouts.Markers;
using NexusMods.DataModel.RateLimiting;
using NexusMods.FileExtractor.Extractors;
using NexusMods.Paths;
using System.Runtime.InteropServices;

namespace NexusMods.CLI;

public static class Services
{
    // ReSharper disable once InconsistentNaming
    public static IServiceCollection AddCLI(this IServiceCollection services)
    {
        services.AddScoped<Configurator>();
        services.AddScoped<CommandLineConfigurator>();
        services.AddSingleton<IOptionParser<AbsolutePath>, AbsolutePathParser>();
        services.AddSingleton<IOptionParser<IGame>, GameParser>();
        services.AddSingleton<IOptionParser<LoadoutMarker>, LoadoutMarkerParser>();
        services.AddSingleton<IOptionParser<Version>, VersionParser>();
        services.AddSingleton<IOptionParser<Loadout>, LoadoutParser>();
        services.AddSingleton<IOptionParser<ITool>, ToolParser>();
        services.AddSingleton<TemporaryFileManager>();
        services.AddSingleton<IProcessFactory, ProcessFactory>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<IProtocolRegistration, ProtocolRegistrationWindows>();
            services.AddSingleton<IOSInterop, OSInteropWindows>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddSingleton<IOSInterop, OSInteropLinux>();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            services.AddSingleton<IOSInterop, OSInteropOSX>();
        }

        services.AddVerb<AnalyzeArchive>()
            .AddVerb<Apply>()
            .AddVerb<ChangeTracking>()
            .AddVerb<ExtractArchive>()
            .AddVerb<ExportLoadout>()
            .AddVerb<FlattenList>()
            .AddVerb<HashFolder>()
            .AddVerb<InstallMod>()
            .AddVerb<ListGames>()
            .AddVerb<ListHistory>()
            .AddVerb<ListManagedGames>()
            .AddVerb<ListModContents>()
            .AddVerb<ListMods>()
            .AddVerb<ListTools>()
            .AddVerb<ManageGame>()
            .AddVerb<ProtocolInvokation>()
            .AddVerb<Rename>()
            .AddVerb<RunTool>();
        
        services.AddAllSingleton<IResource, IResource<IExtractor, Size>>(_ => new Resource<IExtractor, Size>("File Extraction"));
        services.AddAllSingleton<IResource, IResource<FileContentsCache, Size>>(_ => new Resource<FileContentsCache, Size>("File Analysis"));
        return services;
    }
    
}