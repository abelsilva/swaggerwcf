using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace SwaggerWcf.Support
{
    public class StaticContent
    {
        private const string ZipFileName = "SwaggerWcf.www.swagger-ui.zip";
        private static readonly ZipArchive Archive;

        static StaticContent()
        {
            try
            {
                Assembly assembly = Assembly.GetAssembly(typeof(StaticContent));
                Stream zipStream = assembly.GetManifestResourceStream(ZipFileName);

                if (zipStream == null)
                    return;

                Archive = new ZipArchive(zipStream);
            }
            catch
            {
            }
        }

        public static Stream GetFile(string filename, out string contentType, out long contentLength)
        {
            if (Archive == null)
                return ReturnError(out contentType, out contentLength);

            contentType = GetContentType(filename);

            ZipArchiveEntry file = Archive.Entries.FirstOrDefault(entry => entry.FullName == filename);
            if (file != null && contentType != null)
            {
                contentLength = file.Length;
                return file.Open();
            }

            return ReturnError(out contentType, out contentLength);
        }

        private static Stream ReturnError(out string contentType, out long contentLength)
        {
            contentType = "";
            contentLength = 0;
            return Stream.Null;
        }

        private static string GetContentType(string filename)
        {
            int lastIndexOfDot = filename.LastIndexOf('.');
            if (lastIndexOfDot < 0)
                return null;

            string extension = filename.Substring(lastIndexOfDot + 1);
            switch (extension.ToLower())
            {
                case "html":
                    return "text/html";

                case "js":
                    return "application/javascript";

                case "css":
                    return "text/css";

                case "svg":
                    return "image/svg+xml";
                    
                case "ttf":
                    return "application/x-font-ttf";
                    
                case "eot":
                    return "application/vnd.ms-fontobject";

                case "woff":
                    return "application/font-woff";

                case "woff2":
                    return "application/font-woff2";
                    
                case "gif":
                    return "image/gif";
                    
                case "ico":
                    return "image/x-icon";

                case "png":
                    return "image/png";
                    
                default:
                    return null;
            }
        }
    }
}
