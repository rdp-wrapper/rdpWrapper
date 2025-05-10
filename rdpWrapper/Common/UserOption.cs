using System;
using System.Windows.Forms;

namespace sergiye.Common {

  public class UserOption {

    private readonly string name;
    private bool value;
    private readonly ToolStripMenuItem menuItem;
    private event EventHandler OnChanged;
    private readonly PersistentSettings settings;

    public UserOption(string name, bool value, ToolStripMenuItem menuItem, PersistentSettings settings) {
      this.settings = settings;
      this.name = name;
      this.value = name != null ? settings.GetValue(name, value) : value;
      this.menuItem = menuItem;
      this.menuItem.Checked = this.value;
      this.menuItem.Click += MenuItem_Click;
    }

    private void MenuItem_Click(object sender, EventArgs e) {
      Value = !Value;
    }

    public bool Value {
      get => value;
      private set {
        if (this.value == value) return;
        this.value = value;
        if (name != null)
          settings.SetValue(name, value);
        menuItem.Checked = value;
        OnChanged?.Invoke(this, null);
      }
    }

    public event EventHandler Changed {
      add {
        OnChanged += value;
        OnChanged?.Invoke(this, null);
      }
      remove => OnChanged -= value;
    }
  }
}
