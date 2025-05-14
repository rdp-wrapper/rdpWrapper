using sergiye.Common;
using System;
using System.Windows.Forms;

namespace rdpWrapper {
  internal static class Program {

    [STAThread]
    static void Main() {

      if (!OperatingSystemHelper.IsCompatible(true, out var errorMessage, out var fixAction)) {

        if (fixAction != null){
          if (MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
            fixAction?.Invoke();
          }
        }
        else {
          MessageBox.Show(errorMessage, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        Environment.Exit(0);
      }

      if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess) {
        if (MessageBox.Show($"You are running an application build made for a different OS architecture.\nIt is not compatible!\nWould you like to download correct version?", Updater.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
          Updater.VisitAppSite("releases");
        }
        Environment.Exit(0);
      }

      Crasher.Listen();

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
