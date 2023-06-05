using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CodeBase.Utilities.Build.Editor
{
    public class PreBuildResourcesHandler : IPreprocessBuildWithReport
    {
        private const string SpritesFolder = "images";
        private const string FilesListName = "SpitesNames";
        
        public int callbackOrder => 0;

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            // TODO: foreach sub-folders for all resources types
            var resourcesPath = $"{Application.dataPath}/Resources/{SpritesFolder}";
            var fileNames = Directory
                .GetFiles(resourcesPath)
                .Where(x => Path.GetExtension(x) != ".meta")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
            
            var fileInfo = new FileNameInfo(fileNames);
            var fileInfoJson = JsonUtility.ToJson(fileInfo);
            var fileName = $"{Application.dataPath}/Resources/{FilesListName}.json";

            File.WriteAllText(fileName, fileInfoJson);
            AssetDatabase.Refresh();
        }
    }
}