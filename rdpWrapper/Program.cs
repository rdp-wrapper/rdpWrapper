using sergiye.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    private const int CodeException = -1;
    private const int CodeOk = 0;
    private const int CodeInvalidArgs = 1;
    
    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();
    // [DllImport("kernel32.dll")]
    // private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
    // [DllImport("kernel32.dll")]
    // static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine, bool add);
    // delegate Boolean ConsoleCtrlDelegate(uint ctrlType);
    
    private static int result = CodeOk; //lets be a bit optimistic
    private static bool consoleAllocated;

    [STAThread]
    private static void Main(string[] args) {

      Crasher.Listen();

      var consoleMode = args.Length > 0;
      if (consoleMode) {
        if (!AttachConsole(-1)) {
          consoleAllocated = AllocConsole();
        }
        Console.WriteLine($"{Updater.ApplicationTitle} {typeof(Program).Assembly.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}");
      }

      if (!VersionCompatibility.IsCompatible()) {
        const string message = "The application is not compatible with your region.";
        if (consoleMode)
          Console.WriteLine(message);
        else
          MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        if (consoleAllocated) {
          FreeConsole();
        }

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
  }
}
