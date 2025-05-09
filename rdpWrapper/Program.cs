using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    [STAThread]
    static void Main() {

      if (!VersionCompatibility.IsCompatible()) {
        MessageBox.Show("The application is not compatible with your region.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        Environment.Exit(0);
      }

      Crasher.Listen();

      if (!IsVcRedistInstalled("x86") || Environment.Is64BitOperatingSystem && !IsVcRedistInstalled("x64")) {
        if (MessageBox.Show("Microsoft Visual C++ 2015-2022 Redistributable is not installed.\nWould you like to download it now?", Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
          if (Environment.Is64BitOperatingSystem)
            Process.Start("https://aka.ms/vs/17/release/vc_redist.x64.exe");
          else
            Process.Start("https://aka.ms/vs/17/release/vc_redist.x86.exe");
        }
        Environment.Exit(1);
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      using var form = new MainForm();
      form.FormClosed += delegate {
        Application.Exit();
      };
      Application.Run(form);
    }

    private static bool IsVcRedistInstalled(string arch) {
      string registryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\" + arch;
      // for 64-bit OS, check both 32-bit and 64-bit registry views
      var view = (arch == "x64") ? RegistryView.Registry64 : RegistryView.Registry32;
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
      using (var key = baseKey.OpenSubKey(registryKey)) {
        return key != null && key.GetValue("Installed") is int installed && installed == 1;
      }
    }
  }
}
