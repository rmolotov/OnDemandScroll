using System;

namespace CodeBase.Utilities.Build
{
    [Serializable]
    public class FileNameInfo
    {
        public string[] fileNames;

        public FileNameInfo(string[] fileNames) => 
            this.fileNames = fileNames;
    }
}