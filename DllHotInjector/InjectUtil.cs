using System.Diagnostics;
using System.Runtime.InteropServices;

/**
 * A utility class that allow you inject dll to another process
 * 
 */
namespace DllHotInjector
{
    public class InjectUtil
    {
        [DllImport("kernel32.dll")]
        public static extern int VirtualAllocEx(IntPtr hwnd, int lpaddress, int size, int type, int tect);

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr hwnd, int baseaddress, string buffer, int nsize, int filewriten);

        [DllImport("kernel32.dll")]
        public static extern int GetProcAddress(int hwnd, string lpname);

        [DllImport("kernel32.dll")]
        public static extern int GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        public static extern int CreateRemoteThread(IntPtr hwnd, int attrib, int size, int address, int par, int flags, int threadid);


        public static void hotInjectDllTo(string dllName, int pid)
        {
            int baseaddress;
            int temp = 0;
            int procAddr;
            int dlllength = dllName.Length + 1;
            Process target = Process.GetProcessById(pid);

            if (target.Id == pid)
            {
                baseaddress = VirtualAllocEx(target.Handle, 0, dlllength, 4096, 4);
                if (baseaddress == 0)
                {
                    throw new Exception("Failed to alloc mem");
                }

                if (WriteProcessMemory(target.Handle, baseaddress, dllName, dlllength, temp) == 0)
                {
                    throw new Exception("Failed to write mem");
                }

                procAddr = GetProcAddress(GetModuleHandleA("Kernel32"), "LoadLibraryA");
                if (procAddr == 0)
                {
                    throw new Exception("Failed to get enter point");
                }

                if (CreateRemoteThread(target.Handle, 0, 0, procAddr, baseaddress, 0, temp) == 0)
                {
                    throw new Exception("Failed to create listener thread");
                }
                else
                {
                    Console.WriteLine("DLL " + dllName + " Successful injected!");
                }
            }
        }
    }
}
