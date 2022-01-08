// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using AddVcsRootMappingsToIdeAsyncProviderNS;

var projectRootDirectoryPathOption = new Option<string>(
    name: "--project-root-directory-path"
);

var rootCommand = new RootCommand
{
    projectRootDirectoryPathOption
};

rootCommand.SetHandler(
    handle: (
        string projectRootDirectoryPath,
        CancellationToken cancellationToken
    ) => AddVcsRootMappingsToIdeAsyncProvider.AddVcsRootMappingsToIdeAsync(
        projectRootDirectoryInfo: new DirectoryInfo(
            path: projectRootDirectoryPath
        ),
        cancellationToken: cancellationToken
    ),
    projectRootDirectoryPathOption
);

return await rootCommand.InvokeAsync(args: args);