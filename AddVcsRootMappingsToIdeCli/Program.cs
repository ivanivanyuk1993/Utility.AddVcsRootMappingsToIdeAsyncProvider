using System.CommandLine;
using AddVcsRootMappingsToIdeAsyncProviderNS;

var gitRootDirectoryPathOption = new Option<string>(
    name: "--git-root-directory-path"
);

var ideProjectRootDirectoryPathOption = new Option<string>(
    name: "--ide-project-root-directory-path"
);

var rootCommand = new RootCommand
{
    gitRootDirectoryPathOption,
    ideProjectRootDirectoryPathOption
};

rootCommand.SetHandler(
    handle: (
        string gitRootDirectoryPath,
        string ideProjectRootDirectoryPath,
        CancellationToken cancellationToken
    ) => AddVcsRootMappingsToIdeAsyncProvider.AddVcsRootMappingsToIdeAsync(
        gitRootDirectoryInfo: new DirectoryInfo(
            path: gitRootDirectoryPath
        ),
        ideProjectRootDirectoryInfo: new DirectoryInfo(
            path: ideProjectRootDirectoryPath
        ),
        cancellationToken: cancellationToken
    ),
    gitRootDirectoryPathOption,
    ideProjectRootDirectoryPathOption
);

return await rootCommand.InvokeAsync(args: args);