using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    private const int CodeException = -1;
    private const int CodeOk = 0;
    private const int CodeInvalidArgs = 1;
    
    //[DllImport("kernel32.dll")]
    //private static extern bool AttachConsole(int dwProcessId);
    //[DllImport("kernel32.dll")]
    //private static extern bool AllocConsole();
    //[DllImport("kernel32.dll")]
    //private static extern bool FreeConsole();
    // [DllImport("kernel32.dll")]
    // private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
    // [DllImport("kernel32.dll")]
    // static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);
    // delegate Boolean ConsoleCtrlDelegate(uint ctrlType);
    
    private static int result = CodeOk; //lets be a bit optimistic
    //private static bool consoleAllocated;

    [STAThread]
    private static void Main(string[] args) {

      Crasher.Listen();

      var consoleMode = args.Length > 0;
      if (consoleMode) {
        //if (!AttachConsole(-1)) {
        //  consoleAllocated = AllocConsole();
        //}
        Console.WriteLine($"{Updater.ApplicationTitle} {typeof(Program).Assembly.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}");
      }

      if (!OperatingSystemHelper.IsCompatible(true, out var errorMessage, out var fixAction)) {
        if (consoleMode) {
          Console.WriteLine(errorMessage);
        }
        else {
          if (fixAction != null) {
            if (MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
              fixAction?.Invoke();
            }
          }
          else {
            MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
        Environment.Exit(0);
      }

      if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess) {
        if (consoleMode) {
          Console.WriteLine($"You are running an application build made for a different OS architecture.\nIt is not compatible!");
        }
        else {
          if (MessageBox.Show($"You are running an application build made for a different OS architecture.\nIt is not compatible!\nWould you like to download correct version?", Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
            Updater.VisitAppSite("releases");
          }
        }
        Environment.Exit(0);
      }

      if (consoleMode) {
        StartConsole(args);
      }
      else {
        StartWinForms();
      }
      
      Environment.Exit(result);
    }

    private static void StartConsole(string[] args) {
      try {
        // always check for an updated version of the application unless disabled by launch arguments
        // "-o" parameter should not be the first one
        var offline = args.Any(a => a == "-o");
        if (!offline) {
          Updater.CheckForUpdates(true);
        }
        switch (args[0]) {
          case "-h":
            //todo: show help with supported options
            Console.WriteLine("Usage:\n -x \t start UI and no wait for exit;\n -v \t validate Ini;\n -i \t install wrapper;\n -u \t uninstall wrapper");
            break;
          case "-x":
            //start UI as a new separate process
            Process.Start(typeof(Program).Assembly.Location);
            break;
          case "-v":
            //if (wrapper installed & system version is not supported) -> generate new Ini 
            throw new NotImplementedException();
          case "-i": //install
            throw new NotImplementedException();
          case "-u": //uninstall
            throw new NotImplementedException();
          default:
            result = CodeInvalidArgs;
            break;
        }
      }
      catch (Exception ex) {
        Console.WriteLine("Error: " + ex.Message);
        result = CodeException;
      }
      finally {
        // SetConsoleCtrlHandler(null, false);
        // GenerateConsoleCtrlEvent(0, 0);
        // Process.GetCurrentProcess().StandardOutput.Close();
        // Process.GetCurrentProcess().StandardInput.Close();

        //if (consoleAllocated) {
        //  FreeConsole();
        //}

        // Console.Out.Close();
        // Console.Error.Close();
      }
    }
    
    private static void StartWinForms() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      using var form = new MainForm();
      form.FormClosed += delegate {
        Application.Exit();
      };
      Application.Run(form);
    }

    private static bool IsVcRedistInstalled(string arch) {
      var registryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\" + arch;
      var view = (arch == "x64") ? RegistryView.Registry64 : RegistryView.Registry32;
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
      using (var key = baseKey.OpenSubKey(registryKey)) {
        return key != null && key.GetValue("Installed") is int installed && installed == 1;
      }
    }
  }
}
