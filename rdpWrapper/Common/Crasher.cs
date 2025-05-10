using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace sergiye.Common {

  internal static class Crasher {

    public static event EventHandler SaveState;

    public static void Listen(bool disableCrashDialog = true) {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

      if (!disableCrashDialog) return;
      // disable Windows Error Reporting (crash dialog)
      var dwMode = SetErrorMode(ErrorModes.SEM_NOGPFAULT_ERROR_BOX);
      SetErrorMode(dwMode | ErrorModes.SEM_NOGPFAULT_ERROR_BOX);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      if (e.ExceptionObject is Exception exception) {
        var details = exception.TraceException();
        var path = Path.Combine(Path.GetDirectoryName(typeof(Crasher).Assembly.Location), "Crash_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        File.WriteAllText(path, details);
      }

      SaveState?.Invoke(null, EventArgs.Empty);

      Environment.Exit(-1);
    }

    [Flags]
    private enum ErrorModes : uint {
      SYSTEM_DEFAULT = 0x0,
      SEM_FAILCRITICALERRORS = 0x0001,
      SEM_NO_ALIGNMENT_FAULT_EXCEPT = 0x0004,
      SEM_NOGPFAULT_ERROR_BOX = 0x0002,
      SEM_NO_OPEN_FILE_ERROR_BOX = 0x8000
    }

    [DllImport("kernel32.dll")]
    private static extern ErrorModes SetErrorMode(ErrorModes uMode);

    private static string TraceException(this Exception ex) {
      var text = new StringBuilder();
      while (ex != null) {
        text.AppendLine(ex.Message);
        text.AppendLine(ex.GetType().Name);
        text.AppendLine(ex.StackTrace);
        ex = ex.InnerException;
      }
      return text.ToString();
    }
  }
}
