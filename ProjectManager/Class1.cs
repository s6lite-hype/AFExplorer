using System.Diagnostics;
using System;
#pragma warning disable

namespace ProjectManager
{

    public class Timestamp
    {
        public static void New(string text)
        {
            DateTime time = DateTime.Now;
            int hour = time.Hour;
            int minute = time.Minute;
            int second = time.Second;
            int milis = time.Millisecond;
            Console.WriteLine($"[{hour}:{minute}:{second}:{milis}] {text}");
        }
        public static void NewNL(string text)
        {
            DateTime time = DateTime.Now;
            int hour = time.Hour;
            int minute = time.Minute;
            int second = time.Second;
            int milis = time.Millisecond;
            Console.Write($"[{hour}:{minute}:{second}:{milis}] {text}");
        }
    }

    public class Android
    {
        public static void ShellExecute(string filename, string arguments, bool UseShellExecute, bool WaitForExit)
        {
            try
            {
                ProcessStartInfo prc = new()
                {
                    FileName = filename,
                    Arguments = arguments,
                    UseShellExecute = UseShellExecute
                };
                Process pcr = Process.Start(prc);
                if (WaitForExit)
                {
                    pcr.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Timestamp.New($"{ex.Message}");
            }
        }
        public static void MoveFile(string AndroidPath, string WindowsPath, string ADBPath)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Timestamp.New($"{ex.Message}");
            }
        }
        public static void ReadFile(string AndroidPath, string ADBPath) { }
        public static void CopyFile(string AndroidPath, string WindowsPath, string ADBPath) { }
        public static void WriteFile(string AndroidPath, string ADBPath) { }
        public static string ReturnOutputAsString(string adbPath, string command)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(processStartInfo))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        return output;
                    }
                    else
                    {
                        return "Error: Process is null";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

    public static class Logon
    {
        public static void Show()
        {
            Console.WriteLine("                                                 [https://github.com/s6lite_hype/AFExplorer]");
            Console.WriteLine(@"
                                █████╗  ███████╗███████╗██╗  ██╗██████╗ ██╗      ██████╗ ██████╗ ███████╗██████╗ 
                                ██╔══██╗██╔════╝██╔════╝╚██╗██╔╝██╔══██╗██║     ██╔═══██╗██╔══██╗██╔════╝██╔══██╗
                                ███████║█████╗  █████╗   ╚███╔╝ ██████╔╝██║     ██║   ██║██████╔╝█████╗  ██████╔╝
                                ██╔══██║██╔══╝  ██╔══╝   ██╔██╗ ██╔═══╝ ██║     ██║   ██║██╔══██╗██╔══╝  ██╔══██╗
                                ██║  ██║██║     ███████╗██╔╝ ██╗██║     ███████╗╚██████╔╝██║  ██║███████╗██║  ██║
                                ╚═╝  ╚═╝╚═╝     ╚══════╝╚═╝  ╚═╝╚═╝     ╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
                                                                                 ");
        }
    }
}