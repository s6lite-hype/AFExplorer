#pragma warning disable

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using ProjectManager;

class Program
{
    readonly static string adbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "platform-tools", "adb.exe");
    public int OriginalHeight { get; set; }
    public int OriginalWidth { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    private static Program winAdjust;
    public static void Main()
    {
        string settingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "platform-tools", "adb.exe");

        if (!File.Exists(adbPath))
        {
            Console.WriteLine("ADB executable not found in the platform-tools directory.");
            Environment.Exit(1);
        } else {
        winAdjust = new Program();

        winAdjust.OriginalHeight = Console.WindowHeight;
        winAdjust.OriginalWidth = Console.WindowWidth;
        Console.Clear();
        Console.CursorVisible = false;
        NextSteps(settingPath);
        }
    }

    static void ResizeConsoleWindow()
    {
        Timestamp.New("setting resolution...");
        winAdjust.OriginalHeight = Console.WindowHeight;
        winAdjust.OriginalWidth = Console.WindowWidth;

        // Calculate new dimensions
        winAdjust.Width = 144;
        winAdjust.Height = 42;

        // Ensure the buffer size is within the allowable range and larger than the window size
        int bufferWidth = Math.Min(winAdjust.Width + 500, short.MaxValue - 1);
        int bufferHeight = Math.Min(winAdjust.Height + 500, short.MaxValue - 1);

        try
        {
            Console.SetBufferSize(bufferWidth, bufferHeight);
            Console.SetWindowSize(Math.Min(winAdjust.Width, Console.LargestWindowWidth), Math.Min(winAdjust.Height, Console.LargestWindowHeight));
            Timestamp.New("success");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Timestamp.New($"Error setting console resolution: {ex.Message}");
            Environment.Exit(1);
        }
    }

    static void ResetConsoleWindowSize()
    {
        Timestamp.New("resetting resolution...");
        int resetWidth = 120;
        int resetHeight = 30;

        // Ensure the buffer size is within the allowable range and larger than the window size
        int bufferWidth = Math.Max(resetWidth + 2000, Console.WindowWidth);
        int bufferHeight = Math.Max(resetHeight + 2000, Console.WindowHeight);

        try
        {
            Console.SetBufferSize(bufferWidth, bufferHeight);
            Console.SetWindowSize(Math.Min(resetWidth, Console.LargestWindowWidth), Math.Min(resetHeight, Console.LargestWindowHeight));
            Timestamp.New("reset success");
            Console.Clear();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Error resetting console resolution: {ex.Message}");
        }
    }


    static void NextSteps(string adbPath)
    {
        ResizeConsoleWindow(); // Adjust the console window size
        Console.Title = "Android File Explorer - Starting up";
        Console.Clear();
        Logon.Show();
        Console.Clear();
        Logon.Show();
        Timestamp.New(" >  Starting Android Debug Bridge...");

        try
        {
            // Run adb devices command
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = adbPath,
                Arguments = "devices",
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

                    // Parse output to count connected devices
                    int deviceCount = 0;
                    using (StringReader reader = new StringReader(output))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line) && !line.Contains("List of devices attached"))
                            {
                                // Each connected device is represented by a line with the device serial number followed by "device" status
                                deviceCount++;
                            }
                        }
                    }

                    // Output the number of connected devices
                    Timestamp.New(" =-=-=-=-=-=- DEVICE INFO =-=-=-=-=-=\n");
                    Timestamp.New($"   Connected: {deviceCount} device(s)");
                    if (deviceCount == 0)
                    {
                        Console.Title = "Android File Explorer - ECODE 1 : No Device Connected";
                        Timestamp.New(" No device were connected/found, unable to continue");
                        Thread.Sleep(2000);
                        Environment.Exit(1);
                    }
                    else
                    {
                        Timestamp.NewNL("   Model: ");
                        try
                        {
                            Android.ShellExecute(adbPath, "shell getprop ro.product.model", false, true);
                            Console.Title = "Android File Explorer - Reading Device Storage (Internal Only)";
                            Timestamp.New(" >  Reading storage device....");

                            // Display file list and handle user input
                            DisplayFileList(adbPath);
                        }
                        catch (Exception ex)
                        {
                            Timestamp.New(ex.Message);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Timestamp.New($"Error running adb devices: {ex.Message}");
        }
    }

    static void DisplayFileList(string adbPath, string currentPath = "/storage/emulated/0")
    {
        try
        {
            while (true)
            {
                // Execute 'ls' command to get file/folder list
                string output = Android.ReturnOutputAsString(adbPath, $"shell ls -l {currentPath}");

                // Process the output to extract file/folder names and modification dates
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> files = new List<string>();

                // Add the option to go up a directory
                files.Add("..");

                foreach (string line in lines)
                {
                    // Extract folder name and modification date
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 8)
                    {
                        string name = parts[parts.Length - 1];
                        if (parts[0].StartsWith("d"))
                        {
                            files.Add($"[{name}]");
                        }
                        else
                        {
                            files.Add(name);
                        }
                    }
                }

                // Display the file/folder list
                int selectedIndex = 0;
                Console.Clear();
                Logon.Show();

                while (true)
                {
                    // Update the console title with the currently selected item
                    Console.Title = $"Android File Explorer - {currentPath}";

                    // Calculate the padding required for center alignment
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (i == selectedIndex)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(">");
                        }
                        else
                        {
                            Console.Write(" ");
                        }

                        // Print the left arrow and folder name with static padding
                        Console.Write("             "); // Static padding
                        Console.Write(" ");
                        Console.Write(files[i]);
                        Console.ResetColor();
                        Console.WriteLine();
                    }

                    // Display static text options for delete, copy, move at the bottom of the console
                    Console.WriteLine("\n\n    [0] Delete       [1] Move (PC > Android)         [2] Move (Android > PC)         [3] Copy (PC > Android)         [4] Copy (Android > PC)");
                    Console.WriteLine("\n\n   [Dev: s6lite_hype]                                                                                                            [ESC | Exit]");

                    // Handle user input
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        selectedIndex = Math.Max(0, selectedIndex - 1);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        selectedIndex = Math.Min(files.Count - 1, selectedIndex + 1);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        string selectedItem = files[selectedIndex];
                        if (selectedItem == "..")
                        {
                            // Go up one directory level
                            int lastSlashIndex = currentPath.LastIndexOf('/');
                            currentPath = currentPath.Substring(0, lastSlashIndex);
                            if (string.IsNullOrEmpty(currentPath))
                            {
                                currentPath = "/";
                            }
                            break;
                        }
                        else if (selectedItem.StartsWith("[") && selectedItem.EndsWith("]"))
                        {
                            // Enter the selected directory
                            string directoryName = selectedItem.Trim('[', ']');
                            currentPath = $"{currentPath}/{directoryName}";
                            break;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.D0) // 0 = Delete
                    {
                        string selectedItem = files[selectedIndex];
                        HandleDelete(selectedItem, adbPath, currentPath);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.D1) // 3 = Move from PC to Android
                    {
                        string selectedItem = files[selectedIndex];
                        HandleMove(selectedItem, adbPath, currentPath, isAndroidToPc: false);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.D2) // 4 = Move from Android to PC
                    {
                        string selectedItem = files[selectedIndex];
                        HandleMove(selectedItem, adbPath, currentPath, isAndroidToPc: true);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.D3) // 5 = Copy from PC to Android
                    {
                        string selectedItem = files[selectedIndex];
                        HandleCopy(selectedItem, adbPath, currentPath, isAndroidToPc: false);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.D4) // 6 = Copy from Android to PC
                    {
                        string selectedItem = files[selectedIndex];
                        HandleCopy(selectedItem, adbPath, currentPath, isAndroidToPc: true);
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        ResetConsoleWindowSize();
                        Environment.Exit(0);
                    }

                    // Clear the console before redrawing the file/folder list
                    Console.Clear();
                    Logon.Show();
                }
            }
        }
        catch (Exception ex)
        {
            Timestamp.New(ex.Message);
        }
    }

    static void HandleDelete(string selectedItem, string adbPath, string currentPath)
    {
        if (selectedItem == "..") return;

        Console.WriteLine($"\n\nAre you sure you want to delete: {selectedItem}? (y/n)");
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == ConsoleKey.Y)
        {
            Android.ShellExecute(adbPath, $"shell rm -r \"{currentPath}/{selectedItem}\"", false, true);
        }
    }

    static void HandleCopy(string selectedItem, string adbPath, string currentPath, bool isAndroidToPc)
    {
        if (selectedItem == "..") return;

        if (isAndroidToPc)
        {
            // Copy from Android to PC
            Console.WriteLine("\n\nEnter PC destination path:");
            string destination = Console.ReadLine();
            Android.ShellExecute(adbPath, $"pull \"{currentPath}/{selectedItem}\" \"{destination}\"", false, true);
        }
        else
        {
            // Copy from PC to Android
            Console.WriteLine("\n\nEnter PC source path:");
            string source = Console.ReadLine();
            Android.ShellExecute(adbPath, $"push \"{source}\" \"{currentPath}/{selectedItem}\"", false, true);
        }
    }

    static void HandleMove(string selectedItem, string adbPath, string currentPath, bool isAndroidToPc)
    {
        if (selectedItem == "..") return;

        if (isAndroidToPc)
        {
            // Move from Android to PC
            Console.WriteLine("\n\nEnter PC destination path:");
            string destination = Console.ReadLine();
            Android.ShellExecute(adbPath, $"pull \"{currentPath}/{selectedItem}\" \"{destination}\"", false, true);
            Android.ShellExecute(adbPath, $"shell rm -r \"{currentPath}/{selectedItem}\"", false, true);
        }
        else
        {
            // Move from PC to Android
            Console.WriteLine("\n\nEnter PC source path:");
            string source = Console.ReadLine();
            Android.ShellExecute(adbPath, $"push \"{source}\" \"{currentPath}/{selectedItem}\"", false, true);
            File.Delete(source);
        }
    }
}