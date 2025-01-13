using System;
using System.Runtime.InteropServices;

namespace DisableMouseAccelerationCS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Disabling enhanced pointer precision...");
            bool result = MouseAccelerationDisabler.DisableEnhancePointerPrecision();
            if (result)
            {
                Console.WriteLine("Enhanced pointer precision disabled successfully.");
            }
            else
            {
                Console.WriteLine("Failed to disable enhanced pointer precision.");
            }
        }
    }

    public class MouseAccelerationDisabler
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
        private static extern bool SystemParametersInfoGet(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
        private static extern bool SystemParametersInfoSet(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        public const UInt32 SPI_GETMOUSE = 0x0003;
        public const UInt32 SPI_SETMOUSE = 0x0004;
        public const UInt32 SPIF_UPDATEINIFILE = 0x01;
        public const UInt32 SPIF_SENDCHANGE = 0x02;

        public static bool DisableEnhancePointerPrecision()
        {
            int[] mouseParams = new int[3];
            GCHandle handle = GCHandle.Alloc(mouseParams, GCHandleType.Pinned);
            try
            {
                if (!SystemParametersInfoGet(SPI_GETMOUSE, 0, handle.AddrOfPinnedObject(), 0))
                {
                    return false;
                }

                // Set the acceleration value to disable enhanced pointer precision.
                mouseParams[2] = 0;

                // Update the system setting.
                return SystemParametersInfoSet(SPI_SETMOUSE, 0, handle.AddrOfPinnedObject(), SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
    }
}