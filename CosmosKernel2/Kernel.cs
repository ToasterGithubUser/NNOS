﻿using Cosmos.HAL.BlockDevice.Registers;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.ComponentModel.Design;
using Cosmos.HAL.Drivers;
using Cosmos.HAL;
using System.Buffers;
using Cosmos.Core;
using Cosmos.System;
using Console = System.Console;
using PrismAPI;
using PrismAPI.Graphics.Rasterizer;
using PrismAPI.UI;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;

using PrismAPI.UI.Controls;

namespace CosmosKernel1
{


    public class Kernel : Sys.Kernel
    {
        PrismAPI.Graphics.Canvas canvas;
        public static VGAScreen VScreen = new VGAScreen();
        public Display Canvas = null!;
        public Window MyWindow = null!;
        public PrismAPI.UI.Controls.Label MyLabel = null!;

        public void FormatDisk(int index, string format, bool quick = true) { }
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
        private readonly Bitmap bitmap = new Bitmap(10, 10,
                new byte[] { 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255,
                    23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 153, 57, 12, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 153, 57, 12, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72, 72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72,
                    72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255,
                    10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, }, ColorDepth.ColorDepth32);





        private string current_directory;
        private string curren_directory;
        private string directory;
        public string a;

        public static class BootManager
        {
            public static void Boot()
            {


            }

        }
        public static void error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
            Console.BackgroundColor = ConsoleColor.Red;
        }
        public bool IsMBR { get; }


        public static void crash(string reason)
        {
            {

                Console.BackgroundColor = ConsoleColor.Black;
                
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("                             NONAMEOS CRASH REPORTER");
                Console.WriteLine("PANIC!");
                Console.Write(Cosmos.Core.CPU.GetCPUBrandString());
                Console.Write("_");
                Console.Write(Cosmos.Core.CPU.GetCPUVendorName());
                Console.Write("_");
                Console.WriteLine(Cosmos.Core.CPU.GetCPUUptime());
                Console.WriteLine("NoNameOS Milestone 3 has encountered an critical error that cant solve.Error:");
                System.Threading.Thread.Sleep(5000);
                Kernel.error(reason);
                System.Threading.Thread.Sleep(500);

            }
        }

        protected override void BeforeRun()
        {
            MouseManager.ScreenWidth = 640;
            MouseManager.ScreenHeight = 480;
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs, true, true);
            foreach (Disk disk in fs.GetDisks())
            {
                disk.DisplayInformation();
            }

            Console.WriteLine(" _   _       _   _                       ___  ____  ");
            Console.WriteLine("| \\ | | ___ | \\ | | __ _ _ __ ___   ___ / _ \\/ ___| ");
            Console.WriteLine("|  \\| |/ _ \\|  \\| |/ _` | '_ ` _ \\ / _ \\ | | \\___ \\ ");
            Console.WriteLine("| |\\  | (_) | |\\  | (_| | | | | | |  __/ |_| |___) |");
            Console.WriteLine("|_| \\_|\\___/|_| \\_|\\__,_|_| |_| |_|\\___|\\___/|____/ ");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning! this shell is for PROFFESIONAL USE ONLY.");
            Console.WriteLine("Do you want to go in Graphics Mode? (y/n)");
            string inpduta = Console.ReadLine();
            if (inpduta == "Y" || inpduta == "y")
            {
                /*
           You don't have to specify the Mode, but here we do to show that you can.
           To not specify the Mode and pick the best one, use:
           canvas = FullScreenCanvas.GetFullScreenCanvas();
           */
                Canvas = Display.GetDisplay(800, 600);
                MouseManager.ScreenWidth = 800;
                MouseManager.ScreenHeight = 600;

                 // Define the canvas instance.
                MyWindow = new(50, 50, 500, 400, "My Window");
                MyLabel = new(15, 15, $"{Canvas.GetFPS()} FPS");

                MyWindow.Controls.Add(MyLabel);
                WindowManager.Windows.Add(MyWindow);

                // This will clear the canvas with the specified color.
                Canvas.Clear(PrismAPI.Graphics.Color.Blue);
                a = "graphics";
            }
            else
            {
                a = "text";
            }

            Cosmos.Core.CPU.GetCPUBrandString();
            fs.GetDisks();
            System.Threading.Thread.Sleep(1000);

            foreach (Disk disk in fs.GetDisks())
            {
                Console.BackgroundColor = ConsoleColor.Black;
                disk.DisplayInformation();

            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("CPU:");
            Console.Write(Cosmos.Core.CPU.GetCPUBrandString());
            Console.ForegroundColor = ConsoleColor.Green;
            if (!(Cosmos.Core.CPU.GetCPUBrandString() == ""))
            {
                Console.Write("[OK!]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[FAIL!]");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  ");
            Console.Write("RAM:" + GCImplementation.GetAvailableRAM() + "MB Avaliable");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[OK!]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("  ");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Shell Started. Type help for help,thanks a lot to dontsmi1e for code support");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Thanks for using testing branch!");
            Cosmos.Core.CPU.GetCPUBrandString();
            Console.BackgroundColor = ConsoleColor.Black;
            if (File.Exists("0:\\nonameos\\User.cs"))
            {
                Console.WriteLine("Hello," + File.ReadAllText("0:\\nonameos\\User.cs"));
            }
            current_directory = @"0:\";
            if (!File.Exists(@"0:\nonameos\User.cs"))
                curren_directory = @"0:\";
            else
                curren_directory = (File.ReadAllText("0:\\nonameos\\User.cs") + "$" + current_directory);
            if (File.Exists("0:\\nonameos\\UserPassword.cs"))
            {

            E:
                string password = File.ReadAllText(@"0:\\nonameos\\UserPassword.cs");
                Console.Write("Please enter password:");
                string inpdut = Console.ReadLine();
                if (inpdut == password)
                {
                    Console.Clear();
                    Run();
                }
                else
                {

                    if (File.Exists("0:\\nonameos\\UserPassword.cs"))
                    {
                        Console.WriteLine("Unknown password! Try again!");
                        goto E;
                    }
                    else
                    {

                        Run();
                    }
                }
            }
        }


        protected override void Run()
        {

            Kernel kernel = new Kernel();
            current_directory = @"0:\";
            if (a == "graphics")
            {
                try
                {
                    Canvas.Clear(PrismAPI.Graphics.Color.White); // Draw a green background.
                    WindowManager.Update(Canvas);
                    Canvas.DrawCircle((int)MouseManager.X, (int)MouseManager.Y, 10, PrismAPI.Graphics.Color.Black); // Draw the mouse.
                    Canvas.Update();
                }
                catch(Exception e) {
                    Canvas.DrawString(300, 200,e.ToString(), default, PrismAPI.Graphics.Color.White);
                Canvas.Update();
                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                if (!File.Exists(@"0:\nonameos\System.txt"))
                {

                    if (fs.Disks[0].Partitions.Count < 1)
                    {
                        Console.WriteLine("Welcome to NoNameOS! We're doing final touches...");
                        fs.Disks[0].CreatePartition(512);
                        fs.Disks[0].FormatPartition(0, "FAT32", false);
                        Cosmos.Core.ACPI.Reboot();
                    }

                    fs.Initialize(true);
                    Directory.CreateDirectory(@"0:\nonameos\");
                    Console.WriteLine("Creating System Files.....");
                    fs.CreateFile("0:\\nonameos\\System.txt");
                    fs.CreateFile("0:\\nonameos\\User.cs");
                    Console.Write("Please enter your username:");
                    string user = Console.ReadLine();
                    File.WriteAllText("0:\\nonameos\\User.cs", user);

                D:
                    Console.Write("Add password ?(Y/N)");
                    string passwd = Console.ReadLine();
                    if (passwd == "Y" || passwd == "y")
                    {
                        fs.CreateFile("0:\\nonameos\\UserPassword.cs");
                        Console.Write("Enter password for your user");
                        string password = Console.ReadLine();
                        File.WriteAllText(@"0:\nonameos\UserPassword.cs", password);
                    }
                    else if (passwd == "N" || passwd == "n")
                    {

                    }
                    else
                    {
                        goto D;
                    }

                }
                Console.Write(curren_directory);
                string input = Console.ReadLine();
                if (input == "about")
                {

                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("thanks a lot to dontsmi1e from discord for code support");
                    Console.WriteLine("Welcome to NoNameOS 0.1.7 Pre-alpha! build 260: Milestone 4 Codename''");
                    Console.WriteLine("Milestone 2 adds such thing as: File system and commands to interact with it!");
                    Console.WriteLine("Milestone 3 From now milestone 3 adds login screen and setup");
                    Console.WriteLine("Milestone 3.1 :The Git Repo Update! Adds an GitHub repo.");
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                if (input == "help")
                {
                    Console.BackgroundColor = ConsoleColor.Gray;

                    Console.WriteLine("about-about OS                                                                ");
                    Console.WriteLine("shutdown-shutdown OS                                                          ");
                    Console.WriteLine("reboot-reboot OS                                                              ");
                    Console.WriteLine("dir - shows list of files                                                     ");
                    Console.WriteLine("mkdir- makes directory                                                        ");
                    Console.WriteLine("rmdir- deletes directory                                                      ");
                    Console.WriteLine("clear - clears screen                                                         ");
                    Console.WriteLine("cat - read from file (type with.txt ending)                                   ");
                    Console.WriteLine("write-info to file(type file with . ending,then type on 2nd line info to file)");
                    Console.WriteLine("rm - delete file (type with . ending)                                         ");
                    Console.WriteLine("create - Create an file (type with . ending)                                  ");
                    Console.WriteLine("install - formats disk and starts setup                                       ");
                    Console.WriteLine("specs - shows PC specifications                                               ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                if (input == "cd")
                {
                    current_directory = current_directory + input.Remove(3, 0);
                    curren_directory = curren_directory + input.Remove(3, 0);
                }
                if (input == "cd .. ")
                {
                    current_directory = @"0:\";
                }
                if (input == "install")
                {
                    fs.Disks[0].CreatePartition(512);
                    fs.Disks[0].FormatPartition(0, "FAT32", false);
                    Sys.Power.Reboot();
                }
                if (input == "shutdown")
                {

                    Cosmos.Core.ACPI.Shutdown();
                }
                if (input == "reboot")
                {
                    Cosmos.System.Power.Reboot();

                }

                if (input.StartsWith("echo ")) { Console.WriteLine(input.Remove(0, 5)); }

                if (input == "clear")
                {
                    Console.Clear();
                }
                if (input == "cd ")
                {
                    if (Directory.Exists(input.Remove(0, 3)) == true)
                    {
                        current_directory = current_directory + input.Remove(0, 3);
                    }
                    else
                    {
                        Console.WriteLine("not found!");
                    }
                }
                if (input == "dir")
                {
                    var fs_type = fs.GetFileSystemType(@"0:\");
                    string[] filePaths = Directory.GetFiles(current_directory);
                    var drive = new DriveInfo("0");
                    Console.WriteLine("Volume in drive 0 is " + $"{drive.VolumeLabel}");
                    Console.WriteLine("Directory of " + current_directory);
                    Console.WriteLine("\n");
                    for (int i = 0; i < filePaths.Length; ++i)
                    {
                        string path = filePaths[i];
                        Console.WriteLine(System.IO.Path.GetFileName(path));
                    }
                    foreach (var d in System.IO.Directory.GetDirectories(current_directory))
                    {
                        var dir = new DirectoryInfo(d);
                        var dirName = dir.Name;

                        Console.WriteLine(dirName + " <DIR>");
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("        " + $"{drive.TotalSize}" + " bytes");
                    Console.WriteLine("        " + $"{drive.AvailableFreeSpace}" + " bytes free");
                    Console.WriteLine("File System:" + fs_type);
                }
                if (input.StartsWith("mkdir"))
                {
                    {
                        try
                        {
                            Directory.CreateDirectory(@"0:\" + input.Remove(0, 6) + @"\");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                if (input == "error")
                {
                    Kernel.crash("test");
                }
                if (input.StartsWith("rmdir"))
                {
                    {
                        try
                        {
                            Directory.Delete(@"0:\" + input.Remove(0, 6) + @"\");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                    }
                }
                if (input.StartsWith("cat"))
                {
                    {
                        try
                        {
                            Console.WriteLine(File.ReadAllText(@"0:\" + input.Remove(0, 4)));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                if (input.StartsWith("write"))
                {
                    string write = Console.ReadLine();
                    {
                        try
                        {
                            File.WriteAllText(@"0:\" + input.Remove(0, 6), write);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }
                if (input.StartsWith("rm"))
                {
                    if (input.EndsWith(" "))
                    {
                        try
                        {
                            File.Delete(@"0:\" + input.Remove(0, 6));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                    }
                }
                if (input.StartsWith("create"))
                {
                    {
                        try
                        {
                            var file_stream = File.Create(@"0:\" + input.Remove(0, 7));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                    }
                }
                if (input == ("specs"))
                {
                    var fs_type = fs.GetFileSystemType(@"0:\");
                    var drive = new DriveInfo("0");
                    Console.Write("CPU:");
                    Console.WriteLine(Cosmos.Core.CPU.GetCPUBrandString());
                    Console.WriteLine("Drive:");
                    Console.WriteLine("        " + $"{drive.TotalSize}" + " bytes");
                    Console.WriteLine("        " + $"{drive.AvailableFreeSpace}" + " bytes free");
                    Console.WriteLine("File System:" + fs_type);
                    Console.WriteLine("RAM:" + GCImplementation.GetAvailableRAM() + "MB(Avaliable)");
                }
            }
        }
    }

}