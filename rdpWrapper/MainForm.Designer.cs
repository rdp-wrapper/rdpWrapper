using System.Windows.Forms;

namespace rdpWrapper {
  partial class MainForm {
    private System.ComponentModel.IContainer components = null;

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
      this.cbxSingleSessionPerUser = new System.Windows.Forms.CheckBox();
      this.cbxAllowTSConnections = new System.Windows.Forms.CheckBox();
      this.cbDontDisplayLastUser = new System.Windows.Forms.CheckBox();
      this.rgNLAOptions = new System.Windows.Forms.ComboBox();
      this.rgShadowOptions = new System.Windows.Forms.ComboBox();
      this.numRDPPort = new System.Windows.Forms.NumericUpDown();
      this.lRDPPort = new System.Windows.Forms.Label();
      this.cbxHonorLegacy = new System.Windows.Forms.CheckBox();
      this.btnRestartService = new System.Windows.Forms.Button();
      this.gbxGeneralSettings = new System.Windows.Forms.GroupBox();
      this.lblMaxConnections = new System.Windows.Forms.Label();
      this.numMaxConnections = new System.Windows.Forms.NumericUpDown();
      this.cbxAllowAudioCapture = new System.Windows.Forms.CheckBox();
      this.cbxAllowVideoCapture = new System.Windows.Forms.CheckBox();
      this.cbxAllowPnp = new System.Windows.Forms.CheckBox();
      this.cbxAllowPlaybackRedirect = new System.Windows.Forms.CheckBox();
      this.lblShadowMode = new System.Windows.Forms.Label();
      this.lblAuthMode = new System.Windows.Forms.Label();
      this.gbxStatus = new System.Windows.Forms.GroupBox();
      this.btnInstall = new System.Windows.Forms.Button();
      this.btnGenerate = new System.Windows.Forms.Button();
      this.txtServiceVersion = new System.Windows.Forms.TextBox();
      this.lblSupported = new System.Windows.Forms.Label();
      this.lblListenerStateValue = new System.Windows.Forms.Label();
      this.lblServiceStateValue = new System.Windows.Forms.Label();
      this.lblWrapperVersion = new System.Windows.Forms.Label();
      this.lblWrapperStateValue = new System.Windows.Forms.Label();
      this.lblListenerState = new System.Windows.Forms.Label();
      this.lblServiceState = new System.Windows.Forms.Label();
      this.lblWrapperState = new System.Windows.Forms.Label();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.installMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.uninstallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.restartServiceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.generateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.addUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.manageUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.manageUsersoldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.storeSeiingsInFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.wrapperToInstallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showAntivirusWarnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addDefenderExclusionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addFirewallRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.themeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.checkFoNewVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.siteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.txtLog = new sergiye.Common.SimplTextBox();
      ((System.ComponentModel.ISupportInitialize)(this.numRDPPort)).BeginInit();
      this.gbxGeneralSettings.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numMaxConnections)).BeginInit();
      this.gbxStatus.SuspendLayout();
      this.mainMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // cbxSingleSessionPerUser
      // 
      this.cbxSingleSessionPerUser.AutoSize = true;
      this.cbxSingleSessionPerUser.Location = new System.Drawing.Point(6, 65);
      this.cbxSingleSessionPerUser.Name = "cbxSingleSessionPerUser";
      this.cbxSingleSessionPerUser.Size = new System.Drawing.Size(199, 24);
      this.cbxSingleSessionPerUser.TabIndex = 1;
      this.cbxSingleSessionPerUser.Text = "Single session per user";
      // 
      // cbxAllowTSConnections
      // 
      this.cbxAllowTSConnections.AutoSize = true;
      this.cbxAllowTSConnections.Location = new System.Drawing.Point(6, 35);
      this.cbxAllowTSConnections.Name = "cbxAllowTSConnections";
      this.cbxAllowTSConnections.Size = new System.Drawing.Size(210, 24);
      this.cbxAllowTSConnections.TabIndex = 0;
      this.cbxAllowTSConnections.Text = "Enable Remote Desktop";
      // 
      // cbDontDisplayLastUser
      // 
      this.cbDontDisplayLastUser.AutoSize = true;
      this.cbDontDisplayLastUser.Location = new System.Drawing.Point(6, 95);
      this.cbDontDisplayLastUser.Name = "cbDontDisplayLastUser";
      this.cbDontDisplayLastUser.Size = new System.Drawing.Size(239, 24);
      this.cbDontDisplayLastUser.TabIndex = 2;
      this.cbDontDisplayLastUser.Text = "Do not display last username";
      // 
      // rgNLAOptions
      // 
      this.rgNLAOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgNLAOptions.Location = new System.Drawing.Point(276, 118);
      this.rgNLAOptions.Name = "rgNLAOptions";
      this.rgNLAOptions.Size = new System.Drawing.Size(267, 28);
      this.rgNLAOptions.TabIndex = 12;
      // 
      // rgShadowOptions
      // 
      this.rgShadowOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rgShadowOptions.Location = new System.Drawing.Point(276, 172);
      this.rgShadowOptions.Name = "rgShadowOptions";
      this.rgShadowOptions.Size = new System.Drawing.Size(267, 28);
      this.rgShadowOptions.TabIndex = 14;
      // 
      // numRDPPort
      // 
      this.numRDPPort.Location = new System.Drawing.Point(420, 32);
      this.numRDPPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
      this.numRDPPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numRDPPort.Name = "numRDPPort";
      this.numRDPPort.Size = new System.Drawing.Size(120, 26);
      this.numRDPPort.TabIndex = 8;
      this.numRDPPort.Value = new decimal(new int[] {
            3389,
            0,
            0,
            0});
      // 
      // lRDPPort
      // 
      this.lRDPPort.AutoSize = true;
      this.lRDPPort.Location = new System.Drawing.Point(274, 32);
      this.lRDPPort.Name = "lRDPPort";
      this.lRDPPort.Size = new System.Drawing.Size(80, 20);
      this.lRDPPort.TabIndex = 7;
      this.lRDPPort.Text = "RDP Port:";
      // 
      // cbxHonorLegacy
      // 
      this.cbxHonorLegacy.AutoSize = true;
      this.cbxHonorLegacy.Location = new System.Drawing.Point(6, 126);
      this.cbxHonorLegacy.Name = "cbxHonorLegacy";
      this.cbxHonorLegacy.Size = new System.Drawing.Size(253, 24);
      this.cbxHonorLegacy.TabIndex = 3;
      this.cbxHonorLegacy.Text = "Allow to start custom programs";
      // 
      // btnRestartService
      // 
      this.btnRestartService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRestartService.Location = new System.Drawing.Point(443, 18);
      this.btnRestartService.Name = "btnRestartService";
      this.btnRestartService.Size = new System.Drawing.Size(100, 35);
      this.btnRestartService.TabIndex = 3;
      this.btnRestartService.Text = "Restart";
      this.btnRestartService.Click += new System.EventHandler(this.btnRestartService_Click);
      // 
      // gbxGeneralSettings
      // 
      this.gbxGeneralSettings.Controls.Add(this.lblMaxConnections);
      this.gbxGeneralSettings.Controls.Add(this.numMaxConnections);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowAudioCapture);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowVideoCapture);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowPnp);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowPlaybackRedirect);
      this.gbxGeneralSettings.Controls.Add(this.rgShadowOptions);
      this.gbxGeneralSettings.Controls.Add(this.lblShadowMode);
      this.gbxGeneralSettings.Controls.Add(this.rgNLAOptions);
      this.gbxGeneralSettings.Controls.Add(this.lblAuthMode);
      this.gbxGeneralSettings.Controls.Add(this.cbDontDisplayLastUser);
      this.gbxGeneralSettings.Controls.Add(this.lRDPPort);
      this.gbxGeneralSettings.Controls.Add(this.cbxHonorLegacy);
      this.gbxGeneralSettings.Controls.Add(this.numRDPPort);
      this.gbxGeneralSettings.Controls.Add(this.cbxAllowTSConnections);
      this.gbxGeneralSettings.Controls.Add(this.cbxSingleSessionPerUser);
      this.gbxGeneralSettings.Dock = System.Windows.Forms.DockStyle.Top;
      this.gbxGeneralSettings.Location = new System.Drawing.Point(0, 164);
      this.gbxGeneralSettings.Name = "gbxGeneralSettings";
      this.gbxGeneralSettings.Size = new System.Drawing.Size(552, 248);
      this.gbxGeneralSettings.TabIndex = 2;
      this.gbxGeneralSettings.TabStop = false;
      this.gbxGeneralSettings.Text = "General settings";
      // 
      // lblMaxConnections
      // 
      this.lblMaxConnections.AutoSize = true;
      this.lblMaxConnections.Location = new System.Drawing.Point(274, 63);
      this.lblMaxConnections.Name = "lblMaxConnections";
      this.lblMaxConnections.Size = new System.Drawing.Size(132, 20);
      this.lblMaxConnections.TabIndex = 9;
      this.lblMaxConnections.Text = "Max connections:";
      // 
      // numMaxConnections
      // 
      this.numMaxConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.numMaxConnections.Location = new System.Drawing.Point(420, 63);
      this.numMaxConnections.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
      this.numMaxConnections.Name = "numMaxConnections";
      this.numMaxConnections.Size = new System.Drawing.Size(120, 26);
      this.numMaxConnections.TabIndex = 10;
      // 
      // cbxAllowAudioCapture
      // 
      this.cbxAllowAudioCapture.AutoSize = true;
      this.cbxAllowAudioCapture.Location = new System.Drawing.Point(6, 186);
      this.cbxAllowAudioCapture.Name = "cbxAllowAudioCapture";
      this.cbxAllowAudioCapture.Size = new System.Drawing.Size(214, 24);
      this.cbxAllowAudioCapture.TabIndex = 5;
      this.cbxAllowAudioCapture.Text = "Allow client audio capture";
      // 
      // cbxAllowVideoCapture
      // 
      this.cbxAllowVideoCapture.AutoSize = true;
      this.cbxAllowVideoCapture.Location = new System.Drawing.Point(6, 216);
      this.cbxAllowVideoCapture.Name = "cbxAllowVideoCapture";
      this.cbxAllowVideoCapture.Size = new System.Drawing.Size(212, 24);
      this.cbxAllowVideoCapture.TabIndex = 6;
      this.cbxAllowVideoCapture.Text = "Allow client video capture";
      // 
      // cbxAllowPnp
      // 
      this.cbxAllowPnp.AutoSize = true;
      this.cbxAllowPnp.Location = new System.Drawing.Point(274, 216);
      this.cbxAllowPnp.Name = "cbxAllowPnp";
      this.cbxAllowPnp.Size = new System.Drawing.Size(164, 24);
      this.cbxAllowPnp.TabIndex = 15;
      this.cbxAllowPnp.Text = "Allow PNP redirect";
      // 
      // cbxAllowPlaybackRedirect
      // 
      this.cbxAllowPlaybackRedirect.AutoSize = true;
      this.cbxAllowPlaybackRedirect.Location = new System.Drawing.Point(6, 156);
      this.cbxAllowPlaybackRedirect.Name = "cbxAllowPlaybackRedirect";
      this.cbxAllowPlaybackRedirect.Size = new System.Drawing.Size(230, 24);
      this.cbxAllowPlaybackRedirect.TabIndex = 4;
      this.cbxAllowPlaybackRedirect.Text = "Allow host playback redirect";
      // 
      // lblShadowMode
      // 
      this.lblShadowMode.AutoSize = true;
      this.lblShadowMode.Location = new System.Drawing.Point(274, 149);
      this.lblShadowMode.Name = "lblShadowMode";
      this.lblShadowMode.Size = new System.Drawing.Size(193, 20);
      this.lblShadowMode.TabIndex = 13;
      this.lblShadowMode.Text = "Session Shadowing Mode";
      // 
      // lblAuthMode
      // 
      this.lblAuthMode.AutoSize = true;
      this.lblAuthMode.Location = new System.Drawing.Point(277, 95);
      this.lblAuthMode.Name = "lblAuthMode";
      this.lblAuthMode.Size = new System.Drawing.Size(156, 20);
      this.lblAuthMode.TabIndex = 11;
      this.lblAuthMode.Text = "Authentication Mode";
      // 
      // gbxStatus
      // 
      this.gbxStatus.Controls.Add(this.btnRestartService);
      this.gbxStatus.Controls.Add(this.btnInstall);
      this.gbxStatus.Controls.Add(this.btnGenerate);
      this.gbxStatus.Controls.Add(this.txtServiceVersion);
      this.gbxStatus.Controls.Add(this.lblSupported);
      this.gbxStatus.Controls.Add(this.lblListenerStateValue);
      this.gbxStatus.Controls.Add(this.lblServiceStateValue);
      this.gbxStatus.Controls.Add(this.lblWrapperVersion);
      this.gbxStatus.Controls.Add(this.lblWrapperStateValue);
      this.gbxStatus.Controls.Add(this.lblListenerState);
      this.gbxStatus.Controls.Add(this.lblServiceState);
      this.gbxStatus.Controls.Add(this.lblWrapperState);
      this.gbxStatus.Dock = System.Windows.Forms.DockStyle.Top;
      this.gbxStatus.Location = new System.Drawing.Point(0, 33);
      this.gbxStatus.Name = "gbxStatus";
      this.gbxStatus.Size = new System.Drawing.Size(552, 131);
      this.gbxStatus.TabIndex = 1;
      this.gbxStatus.TabStop = false;
      this.gbxStatus.Text = "Diagnostics";
      // 
      // btnInstall
      // 
      this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnInstall.Location = new System.Drawing.Point(443, 55);
      this.btnInstall.Name = "btnInstall";
      this.btnInstall.Size = new System.Drawing.Size(100, 35);
      this.btnInstall.TabIndex = 7;
      this.btnInstall.Text = "Install";
      this.btnInstall.Visible = false;
      this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // btnGenerate
      // 
      this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGenerate.Location = new System.Drawing.Point(443, 92);
      this.btnGenerate.Name = "btnGenerate";
      this.btnGenerate.Size = new System.Drawing.Size(100, 35);
      this.btnGenerate.TabIndex = 11;
      this.btnGenerate.Text = "Generate";
      this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
      // 
      // txtServiceVersion
      // 
      this.txtServiceVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtServiceVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtServiceVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.txtServiceVersion.Location = new System.Drawing.Point(270, 32);
      this.txtServiceVersion.Name = "txtServiceVersion";
      this.txtServiceVersion.ReadOnly = true;
      this.txtServiceVersion.Size = new System.Drawing.Size(163, 19);
      this.txtServiceVersion.TabIndex = 2;
      this.txtServiceVersion.TabStop = false;
      // 
      // lblSupported
      // 
      this.lblSupported.AutoSize = true;
      this.lblSupported.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblSupported.ForeColor = System.Drawing.Color.Red;
      this.lblSupported.Location = new System.Drawing.Point(270, 98);
      this.lblSupported.Name = "lblSupported";
      this.lblSupported.Size = new System.Drawing.Size(131, 20);
      this.lblSupported.TabIndex = 10;
      this.lblSupported.Text = "[not supported]";
      // 
      // lblListenerStateValue
      // 
      this.lblListenerStateValue.AutoSize = true;
      this.lblListenerStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblListenerStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblListenerStateValue.Location = new System.Drawing.Point(150, 98);
      this.lblListenerStateValue.Name = "lblListenerStateValue";
      this.lblListenerStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblListenerStateValue.TabIndex = 9;
      this.lblListenerStateValue.Text = "Unknown";
      // 
      // lblServiceStateValue
      // 
      this.lblServiceStateValue.AutoSize = true;
      this.lblServiceStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblServiceStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblServiceStateValue.Location = new System.Drawing.Point(150, 32);
      this.lblServiceStateValue.Name = "lblServiceStateValue";
      this.lblServiceStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblServiceStateValue.TabIndex = 1;
      this.lblServiceStateValue.Text = "Unknown";
      // 
      // lblWrapperVersion
      // 
      this.lblWrapperVersion.AutoSize = true;
      this.lblWrapperVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperVersion.Location = new System.Drawing.Point(270, 65);
      this.lblWrapperVersion.Name = "lblWrapperVersion";
      this.lblWrapperVersion.Size = new System.Drawing.Size(64, 20);
      this.lblWrapperVersion.TabIndex = 6;
      this.lblWrapperVersion.Text = "1.0.0.0";
      // 
      // lblWrapperStateValue
      // 
      this.lblWrapperStateValue.AutoSize = true;
      this.lblWrapperStateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.lblWrapperStateValue.ForeColor = System.Drawing.Color.Red;
      this.lblWrapperStateValue.Location = new System.Drawing.Point(150, 65);
      this.lblWrapperStateValue.Name = "lblWrapperStateValue";
      this.lblWrapperStateValue.Size = new System.Drawing.Size(83, 20);
      this.lblWrapperStateValue.TabIndex = 5;
      this.lblWrapperStateValue.Text = "Unknown";
      // 
      // lblListenerState
      // 
      this.lblListenerState.AutoSize = true;
      this.lblListenerState.Location = new System.Drawing.Point(12, 98);
      this.lblListenerState.Name = "lblListenerState";
      this.lblListenerState.Size = new System.Drawing.Size(110, 20);
      this.lblListenerState.TabIndex = 8;
      this.lblListenerState.Text = "Listener state:";
      // 
      // lblServiceState
      // 
      this.lblServiceState.AutoSize = true;
      this.lblServiceState.Location = new System.Drawing.Point(12, 32);
      this.lblServiceState.Name = "lblServiceState";
      this.lblServiceState.Size = new System.Drawing.Size(105, 20);
      this.lblServiceState.TabIndex = 0;
      this.lblServiceState.Text = "Service state:";
      // 
      // lblWrapperState
      // 
      this.lblWrapperState.AutoSize = true;
      this.lblWrapperState.Location = new System.Drawing.Point(12, 65);
      this.lblWrapperState.Name = "lblWrapperState";
      this.lblWrapperState.Size = new System.Drawing.Size(114, 20);
      this.lblWrapperState.TabIndex = 4;
      this.lblWrapperState.Text = "Wrapper state:";
      // 
      // mainMenu
      // 
      this.mainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.helpToolStripMenuItem});
      this.mainMenu.Location = new System.Drawing.Point(0, 0);
      this.mainMenu.Name = "mainMenu";
      this.mainMenu.Size = new System.Drawing.Size(552, 33);
      this.mainMenu.TabIndex = 11;
      this.mainMenu.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // testToolStripMenuItem
      // 
      this.testToolStripMenuItem.Name = "testToolStripMenuItem";
      this.testToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
      this.testToolStripMenuItem.Size = new System.Drawing.Size(209, 34);
      this.testToolStripMenuItem.Text = "Test";
      this.testToolStripMenuItem.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(206, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(209, 34);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // toolsToolStripMenuItem
      // 
      this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installMenuItem,
            this.uninstallMenuItem,
            this.restartServiceMenuItem,
            this.generateMenuItem,
            this.toolStripMenuItem3,
            this.addUserToolStripMenuItem,
            this.manageUsersToolStripMenuItem,
            this.manageUsersoldToolStripMenuItem});
      this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
      this.toolsToolStripMenuItem.Size = new System.Drawing.Size(69, 29);
      this.toolsToolStripMenuItem.Text = "Tools";
      // 
      // installMenuItem
      // 
      this.installMenuItem.Name = "installMenuItem";
      this.installMenuItem.Size = new System.Drawing.Size(265, 34);
      this.installMenuItem.Text = "Insrall";
      this.installMenuItem.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // uninstallMenuItem
      // 
      this.uninstallMenuItem.Name = "uninstallMenuItem";
      this.uninstallMenuItem.Size = new System.Drawing.Size(265, 34);
      this.uninstallMenuItem.Text = "Uninstall";
      this.uninstallMenuItem.Click += new System.EventHandler(this.btnInstall_Click);
      // 
      // restartServiceMenuItem
      // 
      this.restartServiceMenuItem.Name = "restartServiceMenuItem";
      this.restartServiceMenuItem.Size = new System.Drawing.Size(265, 34);
      this.restartServiceMenuItem.Text = "Restart service";
      this.restartServiceMenuItem.Click += new System.EventHandler(this.btnRestartService_Click);
      // 
      // generateMenuItem
      // 
      this.generateMenuItem.Name = "generateMenuItem";
      this.generateMenuItem.Size = new System.Drawing.Size(265, 34);
      this.generateMenuItem.Text = "Generate \'wrap.ini\'";
      this.generateMenuItem.Click += new System.EventHandler(this.btnGenerate_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(262, 6);
      // 
      // addUserToolStripMenuItem
      // 
      this.addUserToolStripMenuItem.Name = "addUserToolStripMenuItem";
      this.addUserToolStripMenuItem.Size = new System.Drawing.Size(265, 34);
      this.addUserToolStripMenuItem.Text = "Create RD user";
      this.addUserToolStripMenuItem.Click += new System.EventHandler(this.addUserToolStripMenuItem_Click);
      // 
      // manageUsersToolStripMenuItem
      // 
      this.manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
      this.manageUsersToolStripMenuItem.Size = new System.Drawing.Size(265, 34);
      this.manageUsersToolStripMenuItem.Text = "Manage users";
      this.manageUsersToolStripMenuItem.Click += new System.EventHandler(this.manageUsersToolStripMenuItem_Click);
      // 
      // manageUsersoldToolStripMenuItem
      // 
      this.manageUsersoldToolStripMenuItem.Name = "manageUsersoldToolStripMenuItem";
      this.manageUsersoldToolStripMenuItem.Size = new System.Drawing.Size(265, 34);
      this.manageUsersoldToolStripMenuItem.Text = "Manage users (old)";
      this.manageUsersoldToolStripMenuItem.Click += new System.EventHandler(this.manageUsersoldToolStripMenuItem_Click);
      // 
      // preferencesToolStripMenuItem
      // 
      this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storeSeiingsInFileToolStripMenuItem,
            this.wrapperToInstallMenuItem,
            this.showAntivirusWarnMenuItem,
            this.addDefenderExclusionMenuItem,
            this.addFirewallRuleMenuItem,
            this.toolStripMenuItem4,
            this.themeMenuItem,
            this.showLogToolStripMenuItem});
      this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
      this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(118, 29);
      this.preferencesToolStripMenuItem.Text = "Preferences";
      // 
      // storeSeiingsInFileToolStripMenuItem
      // 
      this.storeSeiingsInFileToolStripMenuItem.Name = "storeSeiingsInFileToolStripMenuItem";
      this.storeSeiingsInFileToolStripMenuItem.Size = new System.Drawing.Size(380, 34);
      this.storeSeiingsInFileToolStripMenuItem.Text = "Store settings in file";
      // 
      // wrapperToInstallMenuItem
      // 
      this.wrapperToInstallMenuItem.Name = "wrapperToInstallMenuItem";
      this.wrapperToInstallMenuItem.Size = new System.Drawing.Size(380, 34);
      this.wrapperToInstallMenuItem.Text = "Preferred wrapper";
      // 
      // showAntivirusWarnMenuItem
      // 
      this.showAntivirusWarnMenuItem.Name = "showAntivirusWarnMenuItem";
      this.showAntivirusWarnMenuItem.Size = new System.Drawing.Size(380, 34);
      this.showAntivirusWarnMenuItem.Text = "Show antivirus warning";
      // 
      // addDefenderExclusionMenuItem
      // 
      this.addDefenderExclusionMenuItem.Name = "addDefenderExclusionMenuItem";
      this.addDefenderExclusionMenuItem.Size = new System.Drawing.Size(380, 34);
      this.addDefenderExclusionMenuItem.Text = "Add Defender exclusion";
      // 
      // addFirewallRuleMenuItem
      // 
      this.addFirewallRuleMenuItem.Name = "addFirewallRuleMenuItem";
      this.addFirewallRuleMenuItem.Size = new System.Drawing.Size(380, 34);
      this.addFirewallRuleMenuItem.Text = "Set firewall rule on port changed";
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(377, 6);
      // 
      // themeMenuItem
      // 
      this.themeMenuItem.Name = "themeMenuItem";
      this.themeMenuItem.Size = new System.Drawing.Size(380, 34);
      this.themeMenuItem.Text = "Themes";
      // 
      // showLogToolStripMenuItem
      // 
      this.showLogToolStripMenuItem.Checked = true;
      this.showLogToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showLogToolStripMenuItem.Name = "showLogToolStripMenuItem";
      this.showLogToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
      this.showLogToolStripMenuItem.Size = new System.Drawing.Size(380, 34);
      this.showLogToolStripMenuItem.Text = "Show log";
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkFoNewVersionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.siteToolStripMenuItem,
            this.aboutToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // checkFoNewVersionToolStripMenuItem
      // 
      this.checkFoNewVersionToolStripMenuItem.Name = "checkFoNewVersionToolStripMenuItem";
      this.checkFoNewVersionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
      this.checkFoNewVersionToolStripMenuItem.Size = new System.Drawing.Size(345, 34);
      this.checkFoNewVersionToolStripMenuItem.Text = "Check fo new version";
      this.checkFoNewVersionToolStripMenuItem.Click += new System.EventHandler(this.checkFoNewVersionToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(342, 6);
      // 
      // siteToolStripMenuItem
      // 
      this.siteToolStripMenuItem.Name = "siteToolStripMenuItem";
      this.siteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
      this.siteToolStripMenuItem.Size = new System.Drawing.Size(345, 34);
      this.siteToolStripMenuItem.Text = "Site";
      this.siteToolStripMenuItem.Click += new System.EventHandler(this.siteToolStripMenuItem_Click);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(345, 34);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // txtLog
      // 
      this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLog.Location = new System.Drawing.Point(0, 412);
      this.txtLog.Name = "txtLog";
      this.txtLog.ReadOnly = true;
      this.txtLog.Size = new System.Drawing.Size(552, 82);
      this.txtLog.TabIndex = 0;
      this.txtLog.Text = "";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(552, 494);
      this.Controls.Add(this.txtLog);
      this.Controls.Add(this.gbxGeneralSettings);
      this.Controls.Add(this.gbxStatus);
      this.Controls.Add(this.mainMenu);
      this.MainMenuStrip = this.mainMenu;
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(574, 3047);
      this.MinimumSize = new System.Drawing.Size(574, 378);
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "RDP Wrapper";
      ((System.ComponentModel.ISupportInitialize)(this.numRDPPort)).EndInit();
      this.gbxGeneralSettings.ResumeLayout(false);
      this.gbxGeneralSettings.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numMaxConnections)).EndInit();
      this.gbxStatus.ResumeLayout(false);
      this.gbxStatus.PerformLayout();
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private CheckBox cbxSingleSessionPerUser;
    private CheckBox cbxAllowTSConnections;
    private CheckBox cbDontDisplayLastUser;
    private ComboBox rgNLAOptions;
    private ComboBox rgShadowOptions;
    private NumericUpDown numRDPPort;
    private Label lRDPPort;
    private CheckBox cbxHonorLegacy;
    private System.Windows.Forms.GroupBox gbxGeneralSettings;
    private Label lblAuthMode;
    private Label lblShadowMode;
    private Button btnRestartService;
    private System.Windows.Forms.GroupBox gbxStatus;
    private Label lblSupported;
    private Label lblListenerStateValue;
    private Label lblServiceStateValue;
    private Label lblWrapperVersion;
    private Label lblWrapperStateValue;
    private Label lblListenerState;
    private Label lblServiceState;
    private Label lblWrapperState;
    private TextBox txtServiceVersion;
    private Button btnGenerate;
    private sergiye.Common.SimplTextBox txtLog;
    private Button btnInstall;
    private MenuStrip mainMenu;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem toolsToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem checkFoNewVersionToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripMenuItem siteToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private ToolStripMenuItem testToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripMenuItem themeMenuItem;
    private ToolStripMenuItem showLogToolStripMenuItem;
    private ToolStripMenuItem installMenuItem;
    private ToolStripMenuItem uninstallMenuItem;
    private ToolStripMenuItem generateMenuItem;
    private ToolStripMenuItem restartServiceMenuItem;
    private ToolStripMenuItem addUserToolStripMenuItem;
    private ToolStripMenuItem manageUsersToolStripMenuItem;
    private ToolStripMenuItem manageUsersoldToolStripMenuItem;
    private ToolStripMenuItem preferencesToolStripMenuItem;
    private ToolStripMenuItem storeSeiingsInFileToolStripMenuItem;
    private ToolStripMenuItem wrapperToInstallMenuItem;
    private ToolStripMenuItem showAntivirusWarnMenuItem;
    private ToolStripMenuItem addDefenderExclusionMenuItem;
    private ToolStripMenuItem addFirewallRuleMenuItem;
    private CheckBox cbxAllowPnp;
    private CheckBox cbxAllowPlaybackRedirect;
    private CheckBox cbxAllowVideoCapture;
    private CheckBox cbxAllowAudioCapture;
    private ToolStripSeparator toolStripMenuItem4;
    private Label lblMaxConnections;
    private NumericUpDown numMaxConnections;
  }
}