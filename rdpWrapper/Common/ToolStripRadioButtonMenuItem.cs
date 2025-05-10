using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace sergiye.Common {

  public class ToolStripRadioButtonMenuItem : ToolStripMenuItem {

    public ToolStripRadioButtonMenuItem()
        : base() {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text)
        : base(text, null, (EventHandler)null) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(Image image)
        : base(null, image, (EventHandler)null) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text, Image image)
        : base(text, image, (EventHandler)null) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text, Image image,
        EventHandler onClick)
        : base(text, image, onClick) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text, Image image,
        EventHandler onClick, string name)
        : base(text, image, onClick, name) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text, Image image,
        params ToolStripItem[] dropDownItems)
        : base(text, image, dropDownItems) {
      Initialize();
    }

    public ToolStripRadioButtonMenuItem(string text, Image image,
        EventHandler onClick, Keys shortcutKeys)
        : base(text, image, onClick) {
      Initialize();
      ShortcutKeys = shortcutKeys;
    }

    private void Initialize() {
      CheckOnClick = true;
    }

    protected override void OnCheckedChanged(EventArgs e) {
      base.OnCheckedChanged(e);

      if (!Checked || Parent == null) return;

      foreach (ToolStripItem item in Parent.Items) {
        if (item is ToolStripRadioButtonMenuItem radioItem
         && radioItem != this
         && radioItem.Checked) {
          radioItem.Checked = false;
          return;
        }
      }
    }

    protected override void OnClick(EventArgs e) {
      if (Checked) return;
      base.OnClick(e);
    }

    protected override void OnPaint(PaintEventArgs e) {
      if (Image != null) {
        base.OnPaint(e);
        return;
      }

      var currentState = CheckState;
      CheckState = CheckState.Unchecked;
      base.OnPaint(e);
      CheckState = currentState;

      var buttonState = RadioButtonState.UncheckedNormal;
      if (Enabled) {
        if (mouseDownState) {
          buttonState = Checked ? RadioButtonState.CheckedPressed : RadioButtonState.UncheckedPressed;
        }
        else if (mouseHoverState) {
          buttonState = Checked ? RadioButtonState.CheckedHot : RadioButtonState.UncheckedHot;
        }
        else {
          if (Checked)
            buttonState = RadioButtonState.CheckedNormal;
        }
      }
      else {
        buttonState = Checked ? RadioButtonState.CheckedDisabled : RadioButtonState.UncheckedDisabled;
      }

      var offset = (ContentRectangle.Height - RadioButtonRenderer.GetGlyphSize(e.Graphics, buttonState).Height) / 2;
      var imageLocation = new Point(ContentRectangle.Location.X + 4, ContentRectangle.Location.Y + offset);

      RadioButtonRenderer.DrawRadioButton(e.Graphics, imageLocation, buttonState);
    }

    private bool mouseHoverState;

    protected override void OnMouseEnter(EventArgs e) {
      mouseHoverState = true;
      Invalidate();
      base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e) {
      mouseHoverState = false;
      base.OnMouseLeave(e);
    }

    private bool mouseDownState;

    protected override void OnMouseDown(MouseEventArgs e) {
      mouseDownState = true;
      Invalidate();
      base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e) {
      mouseDownState = false;
      base.OnMouseUp(e);
    }

    public override bool Enabled {
      get {
        if (!DesignMode && OwnerItem is ToolStripMenuItem {CheckOnClick: true} ownerMenuItem) {
          return base.Enabled && ownerMenuItem.Checked;
        }
        return base.Enabled;
      }
      set => base.Enabled = value;
    }

    protected override void OnOwnerChanged(EventArgs e) {
      if (OwnerItem is ToolStripMenuItem {CheckOnClick: true} ownerMenuItem) {
        ownerMenuItem.CheckedChanged += OwnerMenuItem_CheckedChanged;
      }
      base.OnOwnerChanged(e);
    }

    private void OwnerMenuItem_CheckedChanged(object sender, EventArgs e) {
      Invalidate();
    }
  }
}