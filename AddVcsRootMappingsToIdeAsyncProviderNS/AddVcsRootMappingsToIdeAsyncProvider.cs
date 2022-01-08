using System.Xml;

namespace AddVcsRootMappingsToIdeAsyncProviderNS;

public static class AddVcsRootMappingsToIdeAsyncProvider
{
    public static async Task AddVcsRootMappingsToIdeAsync(
        DirectoryInfo projectRootDirectoryInfo,
        CancellationToken cancellationToken
    )
    {
        var vcsXmlFilePathList = Directory
            .EnumerateFiles(
                path: Path.Combine(
                    path1: projectRootDirectoryInfo.FullName,
                    path2: ".idea"
                ),
                searchPattern: "vcs.xml",
                searchOption: SearchOption.AllDirectories
            )
            .ToArray();
        var directoryToAddList = Directory
            .EnumerateFiles(
                path: projectRootDirectoryInfo.FullName,
                searchPattern: ".git",
                searchOption: SearchOption.AllDirectories
            )
            .Select(selector: gitFilePath =>
            {
                var relativePath = Path.GetRelativePath(
                    path: new FileInfo(fileName: gitFilePath).DirectoryName!,
                    relativeTo: projectRootDirectoryInfo.FullName
                );
                return $"$PROJECT_DIR$/{relativePath}";
            })
            .OrderBy(keySelector: gitFilePath => gitFilePath)
            .ToArray();

        foreach (var vcsFilePath in vcsXmlFilePathList)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename: vcsFilePath);

            var vcsDirectoryMappingsNode = xmlDocument
                .SelectSingleNode(xpath: "/project/component[@name='VcsDirectoryMappings']");
            if (vcsDirectoryMappingsNode == null)
            {
                continue;
            }
            var vcsDirectoryMappingNodeList = vcsDirectoryMappingsNode?
                .SelectNodes(xpath: "mapping");
            if (vcsDirectoryMappingNodeList == null)
            {
                continue;
            }

            var directoryInVcsSet = vcsDirectoryMappingNodeList
                .OfType<XmlNode>()
                .Select(selector: vcsDirectoryMappingNode => vcsDirectoryMappingNode
                    .Attributes?[name: "directory"]?
                    .Value
                )
                .OfType<string>()
                .ToHashSet();

            foreach (var directoryToAdd in directoryToAddList)
            {
                if (!directoryInVcsSet.Contains(item: directoryToAdd))
                {
                    var newMappingXmlElement = xmlDocument.CreateElement(name: "mapping");
                    newMappingXmlElement.SetAttribute(
                        name: "directory",
                        value: directoryToAdd
                    );
                    newMappingXmlElement.SetAttribute(
                        name: "vcs",
                        value: "Git"
                    );
                    vcsDirectoryMappingsNode!.AppendChild(
                        newChild: newMappingXmlElement
                    );
                }
            }

            xmlDocument.Save(filename: vcsFilePath);
        }
    }
}