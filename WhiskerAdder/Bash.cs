using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WhiskerAdder;
public static class Bash
{
    public static void Do(this string cmd)
    {
        cmd = cmd.Replace("\"", "\\\"");

        var DoProc = new ProcessStartInfo();
        DoProc.FileName = "/bin/bash";
        DoProc.Arguments = $"-c \"{cmd}\"";
        DoProc.RedirectStandardOutput = true;
        DoProc.RedirectStandardError = true;
        DoProc.UseShellExecute = false;
        DoProc.CreateNoWindow = true;

        using var StartDoProc = Process.Start(DoProc);
        StartDoProc.WaitForExit();
    }

    public static string GetOutput(this string cmd)
    {
        var process = new ProcessStartInfo();
        process.FileName = "/bin/bash";
        process.Arguments = $"-c \"{cmd}\"";
        process.RedirectStandardOutput = true;
        process.RedirectStandardError = true;
        process.UseShellExecute = false;
        process.CreateNoWindow = true;

        string output = "";
        string error = string.Empty;

        using var StartProc = Process.Start(process);
        StartProc.WaitForExit();

        using (System.IO.StreamReader myOutput = StartProc.StandardOutput)
        {
            output = output + myOutput.ReadToEnd();
        }

        using (System.IO.StreamReader myError = StartProc.StandardError)
        {
            error = error + myError.ReadToEnd();

        }

        if (output.Length > 0) { return output; }
        else { return error; }
    }
}