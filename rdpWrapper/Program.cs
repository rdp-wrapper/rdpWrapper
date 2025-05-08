using sergiye.Common;
using System;
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
