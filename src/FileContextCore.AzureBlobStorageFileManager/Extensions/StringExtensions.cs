using System.IO;

namespace FileContextCore.AzureBlobStorageFileManager.Extensions
{
    static class StringExtensions
    {
        public static string GetValidFileName(this string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (c == '\\' || c == '/')
                {
                    continue;
                }

                input = input.Replace(c, '_');
            }

            return input;
        }
    }
}
