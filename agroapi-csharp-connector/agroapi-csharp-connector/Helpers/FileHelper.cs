using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SciCrop.AgroAPI.Connector.Helpers
{
    public class FileHelper
    {

        private static FileHelper instance;

        private FileHelper() { }

        public static FileHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileHelper();
                }
                return instance;
            }
        }

        public string GetStringFromFilePath(string filePath)
        {
            string fileStr = null;

            if (File.Exists(filePath))
            {
                fileStr = File.ReadAllText(filePath);
            }

            return fileStr;
        }
    }
}
