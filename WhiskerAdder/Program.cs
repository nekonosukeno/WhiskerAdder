using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static WhiskerAdder.Bash;
using static WhiskerAdder.PathTool;

namespace WhiskerAdder
{

    public static class Exit
    {
        public static void Msg()
        {
            Console.WriteLine("\nWhisker Adder v0.2rc by Nekonosuke");
            Console.WriteLine("Create \"start menu\" shortcuts for executable binaries, both Linux and Windows/Wine");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("    -w, --wine        specify a Windows executable");
            Console.WriteLine("    -pfx, --prefix    specify a wine prefix to use");
            Console.WriteLine("    -h, --help        display this help message\n");
            Console.WriteLine("Useage / Switch Order:");
            Console.WriteLine("./WhiskerAdder -w --prefix [/path/to/pfx] [/path/to/binary.exe]\n");
            Console.WriteLine("Hints and Reminders:");
            Console.WriteLine("    -Use the full file paths from root");
            Console.WriteLine("    -Most terminal emulators allow drag and drop of files\n");
            Environment.Exit(1);
        }   
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            bool ExtractIcon = false;
            string ErrMsg = "ERROR: ";
            string invlpth = " is a valid path but ";
            string invlfile = " is not a valid file";
            string Entry = "[Desktop Entry]";
            string Exec = "Exec=";
            string Path = "Path=";
            string Type = "Type=Application";
            string Icon = "Icon=";
            string LastArg = null;
            if (args.Length > 0) { LastArg = args[args.Length - 1]; }
            else
            {
                Console.WriteLine("Enter path to executable binary:");
                LastArg = Console.ReadLine().Replace("\'", "");
                if (LastArg.EndsWith(" ")) { LastArg = LastArg.TrimEnd(' '); }
                Console.WriteLine(LastArg);
            }
            string ExeName = PathTool.FileName(LastArg);
            string GetPath = PathTool.rmName(LastArg);
            Path = $"{Path}\"{GetPath}\"";
            List<string> Contents = new List<string>();

            // Fishing for errors...
            // Some broad level exceptions (help text or invalid file)
            if (args.Length > 0 && (args[0] == "-h" ^ args[0] == "--help"))
            {
                Exit.Msg();
            }

            if (!File.Exists(@LastArg))
            {
                if (Directory.Exists(GetPath))
                {
                    Console.WriteLine(LastArg);
                    ErrMsg += $"\"{GetPath}\"{invlpth}\"{ExeName}\"{invlfile}";
                }
                else
                {
                    ErrMsg += $"\"{GetPath}\" is not a valid path. Cannot find file.";
                }

                Console.WriteLine(ErrMsg);
                Exit.Msg();
            }

            // Incorrect argument(s) given
            switch (args.Length)
            {
                case > 4:
                    Console.WriteLine($"{ErrMsg}too many arguments");
                    Exit.Msg();
                    break;
                case 4:
                    if (args[0] != "-w" && args[0] != "--wine")
                    {
                        Console.WriteLine($"{ErrMsg}\"{args[0]}\" is not a valid argument");
                        Exit.Msg();
                    }

                    if (args[1] != "-pfx" && args[1] != "--prefix")
                    {
                        Console.WriteLine($"{ErrMsg}\"{args[1]}\" is not a valid argument");
                        Exit.Msg();
                    }

                    if (!Directory.Exists(@args[2]))
                    {
                        Console.WriteLine($"{ErrMsg}\"{args[2]}\" is not a valid path");
                        Exit.Msg();
                    }

                    break;
                case 3:
                    Console.WriteLine($"{ErrMsg}missing an argument");
                    Exit.Msg();
                    break;
                case 2:
                    if (args[0] != "-w" && args[0] != "--wine")
                    {
                        Console.WriteLine($"{ErrMsg}\"{args[0]}\" is not a valid argument");
                        Exit.Msg();
                    }

                    break;
            }

            // If you made it this far, your arguments must be valid
            // Determining Exec line
            switch (args.Length)
            {
                case 0:
                case 1:
                    if (LastArg.EndsWith(".exe") || LastArg.EndsWith(".EXE"))
                    {
                        Console.WriteLine("Enter Wine prefix (leave blank for default):");
                        string GetPfx = Console.ReadLine();
                        while (GetPfx.Length > 0 && !Directory.Exists(GetPfx))
                        {
                            Console.WriteLine("The prefix path you have entered is invalid");
                            Console.WriteLine("Enter Wine prefix (leave blank for default):");
                            GetPfx = Console.ReadLine();
                        }
                        if (GetPfx.Length > 0) { Exec += $"env WINEPREFIX=\'{GetPfx}\' "; }
                        Exec += $"wine \'{LastArg}\'";
                        ExtractIcon = true;
                    }

                    if (!LastArg.EndsWith(".exe") && !LastArg.EndsWith(".EXE"))
                    {
                        string WriteExec = $"{GetPath}./{ExeName}";
                        Exec += $"\'{WriteExec}\'";
                    }

                    break;
                case 2:
                    if (args[0] == "-w" || args[0] == "--wine")
                    {
                        Exec += $"wine \'{LastArg}/'";
                        ExtractIcon = true;
                    }

                    break;
                case 4:
                    if (args[1] == "-pfx" || args[1] == "--wineprefix")
                    {
                        Exec += $"env WINEPREFIX=\'{args[2]}\' wine \'{LastArg}\'";
                        ExtractIcon = true;
                    }

                    break;
            }

            // Ask App name
            Console.WriteLine("Enter application name:");
            string LauncherName = Console.ReadLine();
            string Name = "Name=" + LauncherName;

            // Get or ask for Icon
            if (ExtractIcon)
            {
                string t14Name = IconTool.FindIcon(LastArg);
                if (t14Name.Length > 0) { Icon += IconTool.SaveIcon(LastArg, t14Name); }
                else { Icon += IconTool.AskPath(); }
            }
            else { Icon += IconTool.AskPath(); }

            // Keywords to search for app
            Console.WriteLine("Enter keywords separated by a space:");
            string Keys = Console.ReadLine();
            Keys = Keys.Replace(" ", ";");
            Keys = "Keywords=" + Keys + ";";

            // Add everything to list
            Contents.Add(Entry);
            Contents.Add(Exec);
            Contents.Add(Name);
            Contents.Add(Path);
            Contents.Add(Type);
            if ( Icon.Length > 5 ) { Contents.Add(Icon); }
            if ( Keys.Length > 10 ) { Contents.Add(Keys); }

            string Home = Environment.GetEnvironmentVariable("HOME");
            string LauncherFile = Home + "/.local/share/applications/" + LauncherName + ".desktop";
            // Console.WriteLine(LauncherFile);
            using (StreamWriter Launcher = new StreamWriter(LauncherFile, false))
            {
                Launcher.Write(Contents[0]);
                for (int i = 1; i < Contents.Count; i++)
                {
                    Launcher.Write("\n" + Contents[i]);
                }
            }

            Bash.Do($"chmod +x \'{LauncherFile}\'");
            if (File.Exists(LauncherFile))
            {
                string SavedTo = PathTool.rmName(LauncherFile);
                Console.WriteLine($"\nYour .desktop file has been saved to \'{SavedTo}\'\n");   
            }
        }
    }
}