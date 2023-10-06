using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WhiskerAdder;
public static class PathTool
{
    // Returns path to a file but without the file name
    public static string rmName(string Path)
    {
        string[] PathSplit = Path.Split(new char[] { '/', '\\' });
        string NewPath;

        if (Path.Contains("/")) { NewPath = String.Join("/", PathSplit, 0, PathSplit.Length - 1) + "/"; }
        else { NewPath = String.Join("\\", PathSplit, 0, PathSplit.Length - 1) + "\\"; }

        return NewPath;
    }

    // Gets the base file name without the path
    public static string FileName(string Path)
    {
        string[] PathSplit = Path.Split(new char[] { '/', '\\' });
        string BaseName = PathSplit[PathSplit.Length - 1];
        return BaseName;
    }

    // Unix to Windows/Wine. Assumes full path, not local
    public static string u2w(string Path)
    {
        string NewPath;
        if (!Path.StartsWith('/')) { NewPath = "Z:\\" + Path.Replace('/', '\\'); }
        else { NewPath = "Z:" + Path.Replace('/', '\\'); }

        return NewPath;
    }

    // Windows/Wine to Unix. Assumes full path, not local
    public static string w2u(string Path)
    {
        string SlashPath = Path.Replace('\\', '/');
        string NewPath = SlashPath.Substring(2, SlashPath.Length - 2);
            
        return NewPath;
    }
}
