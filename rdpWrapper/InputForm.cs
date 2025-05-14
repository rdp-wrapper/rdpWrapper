using sergiye.Common;
using System.Drawing;
using System.Windows.Forms;

namespace rdpWrapper {

  public partial class InputForm : Form {

    public InputForm() {
      InitializeComponent();
      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);

      txtInput.TextChanged += (s, e) => btnOk.Enabled = !string.IsNullOrEmpty(txtInput.Text);
    }

    public static DialogResult GetValue(string title, string promptText, out string input) {

      var form = new InputForm();
      form.lblText.Text = promptText;
      form.Text = title;
      Theme.Current.Apply(form);
      var dialogResult = form.ShowDialog();
      input = form.txtInput.Text;
      return dialogResult;
    }
  }
}
