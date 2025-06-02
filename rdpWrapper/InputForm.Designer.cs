namespace rdpWrapper {
  partial class InputForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.txtInput = new System.Windows.Forms.TextBox();
      this.lblText = new System.Windows.Forms.Label();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtInput
      // 
      this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtInput.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.txtInput.Location = new System.Drawing.Point(18, 63);
      this.txtInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.txtInput.Name = "txtInput";
      this.txtInput.Size = new System.Drawing.Size(393, 37);
      this.txtInput.TabIndex = 1;
      // 
      // lblText
      // 
      this.lblText.AutoSize = true;
      this.lblText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblText.Location = new System.Drawing.Point(13, 22);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(112, 28);
      this.lblText.TabIndex = 0;
      this.lblText.Text = "Enter value:";
      // 
      // btnOk
      // 
      this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Enabled = false;
      this.btnOk.Location = new System.Drawing.Point(138, 124);
      this.btnOk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(129, 43);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(296, 124);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(110, 43);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // InputForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(420, 180);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.lblText);
      this.Controls.Add(this.txtInput);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(1109, 236);
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(442, 236);
      this.Name = "InputForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.Label lblText;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
  }
}