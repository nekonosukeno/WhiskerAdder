# WhiskerAdder  
  
Named for the XFCE Whisker Menu but should work on virtually any Linux Desktop Environment and distro.  
Runs via command line and works for native or Windows applications.(CLI)  
  
Creates a "start menu" entry for a given program. It attempts to remove as much guesswork as possible  
to get your entry added quickly and easily. If the program is an ".exe" Windows application,  
it will attempt to extract and use the correct icon if it has one.  
  
**Useage / Switch Order:**  
For native applications:  
./WhiskerAdder </path/to/binary>  
  
For Wine applications:  
./WhiskerAdder -w [--prefix] [/path/to/pfx] </path/to/binary.exe>  
  
If you have not decided yet:  
./WhiskerAdder  
  
**Options:**  
    -w, --wine        specify a Windows executable  
    -pfx, --prefix    specify a wine prefix to use  
    -h, --help        display this help message  
**Hints and Reminders:**  
    -Use the full file paths from root  
    -Most terminal emulators allow drag and drop of files  
