using System;

namespace VladislavTsurikov.Scripts 
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LocationAttribute : Attribute
    {
        private readonly string _relativePath;
        private string _filePath;

        public string RelativePath => _relativePath;
        public string FilePath 
        {
            get 
            {
                if (_filePath != null) return _filePath;
                
                string pathToFolder = CommonPath.CombinePath(CommonPath.PathToResources, _relativePath);
                
                _filePath = pathToFolder + ".asset";

                return _filePath;
            }
        }

        public LocationAttribute(string relativePath)
        {
            _relativePath = relativePath;
        }
    }
}