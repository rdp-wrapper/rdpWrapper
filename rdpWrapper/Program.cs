using Microsoft.Win32;
using sergiye.Common;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    [STAThread]
    static void Main() {

      if (!OperatingSystemHelper.IsCompatible()) {
        MessageBox.Show("The application is not compatible with your region.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        Environment.Exit(0);
      }

      Crasher.Listen();

      var sysArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
      var appArch = Environment.Is64BitProcess ? "x64" : "x86";
      if (sysArch != appArch) {
        var answer = MessageBox.Show($"You are running {appArch} application on {sysArch} OS.\nIt may not be compatible!\nWould you like to download correct version?", Updater.ApplicationName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
        switch (answer) {
          case DialogResult.Yes:
            Updater.VisitAppSite("releases");
            return;
          case DialogResult.No:
            break;
          case DialogResult.Cancel:
            return;
        }
      }

      if (!IsVcRedistInstalled(sysArch)) {
        if (MessageBox.Show("Microsoft Visual C++ 2015-2022 Redistributable is not installed.\nWould you like to download it now?", Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
          Process.Start($"https://aka.ms/vs/17/release/vc_redist.{sysArch}.exe");
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
      var registryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\" + arch;
      var view = (arch == "x64") ? RegistryView.Registry64 : RegistryView.Registry32;
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
      using (var key = baseKey.OpenSubKey(registryKey)) {
        return key != null && key.GetValue("Installed") is int installed && installed == 1;
      }
    }
  }
}
