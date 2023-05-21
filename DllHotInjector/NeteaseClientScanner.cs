using System.Diagnostics;
using System.Management;

/**
 * From jsmhToolChest.
 */
namespace Core
{
    public class NeteaseClientScanner
    {
        public static Process GetNeteaseClientProcess()
        {
            try
            {
                Process[] ProcessList = Process.GetProcesses();
                foreach (Process EachProcess in ProcessList)
                {
                    if (EachProcess.ProcessName.Equals("java") || EachProcess.ProcessName.Equals("javaw"))
                    {
                        string query = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {EachProcess.Id}";

                        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                        {
                            foreach (ManagementObject obj in searcher.Get())
                            {
                                if (obj["CommandLine"] != null)
                                {
                                    string CommandLine = obj["CommandLine"].ToString();
                                    if (CommandLine.Contains("-DlauncherControlPort") || CommandLine.Contains("-DlauncherGameId"))
                                    {
                                        return EachProcess;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}
