using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static WhiskerAdder.Bash;
using static WhiskerAdder.PathTool;

namespace WhiskerAdder;
    public static class IconTool
    {
        public static string FindIcon(string LastArg)
        {
            // Listing Icons in .exe
            string command = $"wrestool -l -t14 \'{LastArg}\'";
            // Console.WriteLine("Running:\n" + command);
            string ShowOutput = Bash.GetOutput(command);
            string t14Name = null;
            string[] SplitOut = ShowOutput.Split("--type=14 --name=", StringSplitOptions.RemoveEmptyEntries);
            if ( ShowOutput.Length > 0 && SplitOut.Length > 1 )
            {
                List<int> IconSizes = new List<int>();
                for (int i = 1; i < SplitOut.Length; i++)
                {
                    string[] GetSize = SplitOut[i].Split("size=");
                    string SizeStr = GetSize[1].Substring(0, GetSize[1].Length - 2);
                    int Size = Convert.ToInt32(SizeStr);
                    IconSizes.Add(Size);
                }
                // Finding icon entry with the largest size
                int Bingo = IconSizes.IndexOf(IconSizes.Max());
                t14Name = SplitOut[Bingo].Split(" --lang", StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else if (ShowOutput.Length > 0 && SplitOut.Length == 1)
            {
                t14Name = ShowOutput.Split(" --lang", StringSplitOptions.RemoveEmptyEntries)[0];
            }
            t14Name = t14Name.Split("--type=14 --name=", StringSplitOptions.RemoveEmptyEntries)[0];
            return t14Name;
        }

        public static string SaveIcon(string LastArg, string t14Name)
        {
            string IconName = PathTool.FileName(LastArg).Split('.')[0] + ".ico";
            string IconFileUnix = PathTool.rmName(LastArg) + IconName;
            // Saving Icon to same place as .exe
            string SaveIcon = $"wrestool -x \'{LastArg}\' -n {t14Name} -o \'{IconFileUnix}\'";
            // Console.WriteLine(SaveIcon);
            Bash.Do(SaveIcon);
            // Console.WriteLine("Finished");
            return IconFileUnix;
        }

        public static string AskPath()
        {
            Console.WriteLine("Enter icon path (leave blank if none):");
            string getIcon = Console.ReadLine().Replace("\'", "");
            if (getIcon.EndsWith(" ")) { getIcon = getIcon.TrimEnd(' '); }

            while (getIcon.Length > 0 && !File.Exists(@getIcon))
            {
                Console.WriteLine("The icon you have entered does not exist");
                Console.WriteLine("Enter icon path (leave blank if none):");
                getIcon = Console.ReadLine().Replace("\'", "");
                if (getIcon.EndsWith(" ")) { getIcon = getIcon.TrimEnd(' '); }
            }

            return getIcon;
        }
    }