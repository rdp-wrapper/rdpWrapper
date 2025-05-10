using sergiye.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace rdpWrapper {

  internal partial class MainForm : Form {

    private int oldPort;
    private string wrapperIniLastPath;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported;
    private readonly Timer refreshTimer;
    private readonly Logger logger;
    private readonly Wrapper wrapper;
    private readonly PersistentSettings settings;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      var title =$"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}";
      Text = title;

      settings = new PersistentSettings();
      settings.Load();
      InitializeTheme();

      var sizeSpan = Height - ClientSize.Height;
      MinimumSize = new Size { Width = Width, Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + mainMenu.Height };

      var showLog = new UserOption("showLog", false, showLogToolStripMenuItem, settings);
      showLog.Changed += delegate {
        showLogToolStripMenuItem.Checked = showLog.Value;
        if (showLogToolStripMenuItem.Checked) {
          FormBorderStyle = FormBorderStyle.Sizable;
          txtLog.Visible = true;
          Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + 100 + mainMenu.Height;
        }
        else {
          FormBorderStyle = FormBorderStyle.FixedDialog;
          Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + mainMenu.Height;
          txtLog.Visible = false;
        }
      };

      logger = new Logger();
      logger.OnNewLogEvent += AddToLog;
      logger.Log($"Application started: {title}", Logger.StateKind.Info, false);
      
      wrapper = new Wrapper(logger);
      
      rgNLAOptions.Items.AddRange([
        "GUI Authentication Only", 
        "Default RDP Authentication", 
        "Network Level Authentication" 
      ]);
      rgShadowOptions.Items.AddRange([
        "Disable Shadowing",
        "Full access with user's permission",
        "Full access without permission",
        "View only with user's permission",
        "View only without permission"
      ]);

      Load += (sender, e) => { 
        RefreshSystemSettings();
      };

      refreshTimer = new Timer();
      refreshTimer.Tick += TimerTick;
      refreshTimer.Interval = 1000;

      var timer = new Timer();
      timer.Tick += async (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !await Updater.CheckForUpdatesAsync(true);
      };
      timer.Interval = 3000;
      timer.Enabled = true;
    }

    private void checkFoNewVersionToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.CheckForUpdates(false);
    }

    private void siteToolStripMenuItem_Click(object sender, EventArgs e) {
      Updater.VisitAppSite();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
      MessageBox.Show($"{Updater.ApplicationTitle} {Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x32")}\nWritten by Sergiy Egoshyn (egoshin.sergey@gmail.com)", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void RefreshSystemSettings() {
      try {
        logger.Log("Retrieving system configuration...");

        cbxSingleSessionPerUser.Checked = wrapper.SingleSessionPerUser;
        cbxAllowTSConnections.Checked = wrapper.AllowTsConnections;
        cbxHonorLegacy.Checked = wrapper.HonorLegacy;
        seRDPPort.Value = oldPort = wrapper.RdpPort;
        
        rgNLAOptions.SelectedIndex = wrapper.SecurityLayer switch {
          0 when wrapper.UserAuthentication == 0 => 0,
          1 when wrapper.UserAuthentication == 0 => 1,
          2 when wrapper.UserAuthentication == 1 => 2,
          _ => rgNLAOptions.SelectedIndex
        };

        rgShadowOptions.SelectedIndex = wrapper.ShadowOptions;
        cbDontDisplayLastUser.Checked = wrapper.DontDisplayLastUser;

        logger.Log(" Done", Logger.StateKind.Info, false);
        TimerTick(null, EventArgs.Empty);
        refreshTimer.Enabled = true;
      }
      catch (Exception ex) {
        var message = "Error loading settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        //MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      btnApply.Enabled = false;
    }

    private void btnClose_Click(object sender, EventArgs e) {
      Close();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      if (btnApply.Enabled && MessageBox.Show("Settings are not saved. Do you want to exit?", Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) {
        e.Cancel = true;
      }
    }

    private void InitializeTheme() {

      mainMenu.Renderer = new ThemedToolStripRenderer();

      if (Theme.SupportsAutoThemeSwitching()) {
        var autoThemeMenuItem = new ToolStripRadioButtonMenuItem("Auto");
        autoThemeMenuItem.Click += (o, e) => {
          autoThemeMenuItem.Checked = true;
          Theme.SetAutoTheme();
          settings.SetValue("theme", "auto");
        };
        themeMenuItem.DropDownItems.Add(autoThemeMenuItem);
      }

      var setTheme = Theme.All.FirstOrDefault(theme => settings.GetValue("theme", "auto") == theme.Id);
      if (setTheme != null) {
        Theme.Current = setTheme;
        Theme.Current.Apply(this);
      }
      else {
        themeMenuItem.DropDownItems[0].PerformClick();
      }

      AddThemeMenuItems(Theme.All.Where(t => t is not CustomTheme));
      var customThemes = Theme.All.Where(t => t is CustomTheme).ToList();
      if (customThemes.Count > 0) {
        themeMenuItem.DropDownItems.Add("-");
        AddThemeMenuItems(customThemes);
      }
    }

    private void AddThemeMenuItems(IEnumerable<Theme> themes) {
      foreach (var theme in themes) {
        var item = new ToolStripRadioButtonMenuItem(theme.DisplayName);
        item.Tag = theme;
        item.Click += OnThemeMenuItemClick;
        themeMenuItem.DropDownItems.Add(item);

        if (Theme.Current != null && Theme.Current.Id == theme.Id) {
          item.Checked = true;
        }
      }
    }

    private void OnThemeMenuItemClick(object sender, EventArgs e) {
      if (sender is not ToolStripRadioButtonMenuItem item || item.Tag is not Theme theme)
        return;
      item.Checked = true;
      Theme.Current = theme;
      settings.SetValue("theme", theme.Id);
    }

    private void btnApply_Click(object sender, EventArgs e) {
      
      try {
        wrapper.SingleSessionPerUser = cbxSingleSessionPerUser.Checked;
        wrapper.AllowTsConnections = cbxAllowTSConnections.Checked;
        wrapper.HonorLegacy = cbxHonorLegacy.Checked;
        
        var newPort = (int)seRDPPort.Value;
        if (oldPort != newPort) {
          var p = Wrapper.StartProcess("netsh", $"advfirewall firewall set rule name=\"Remote Desktop\" new localport={newPort}");
          p.WaitForExit();
          logger.Log($"Firewall rule added for port {newPort}", Logger.StateKind.Info);
        }

        oldPort = wrapper.RdpPort = newPort;
        switch (rgNLAOptions.SelectedIndex) {
          case 0:
            wrapper.UserAuthentication = 0;
            wrapper.SecurityLayer = 0;
            break;
          case 1:
            wrapper.UserAuthentication = 0;
            wrapper.SecurityLayer = 1;
            break;
          case 2:
            wrapper.UserAuthentication = 1;
            wrapper.SecurityLayer = 2;
            break;
        }
        wrapper.ShadowOptions = rgShadowOptions.SelectedIndex;
        wrapper.DontDisplayLastUser = cbDontDisplayLastUser.Checked;

        wrapper.SaveSettings();
        
        btnApply.Enabled = false;
        logger.Log("Settings applied", Logger.StateKind.Info);
      }
      catch (Exception ex) {
        var message = "Failed to apply settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void OnChanged(object sender, EventArgs e) {
      btnApply.Enabled = true;
    }

    private void btnRestartService_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        //todo: async
        wrapper.StopService(TimeSpan.FromSeconds(10));
        wrapper.StartService(TimeSpan.FromSeconds(10));
      }
      catch (Exception ex) {
        var message = "Error restarting service: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void TimerTick(object sender, EventArgs e) {

      bool? checkSupported = false;
      var wrapperInstalled = wrapper.CheckWrapperInstalled();
      switch (wrapperInstalled) {
        case WrapperInstalledState.Unknown:
          lblWrapperStateValue.Text = "Unknown";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusInfoColor;
          installMenuItem.Enabled = btnInstall.Visible = false;
          uninstallMenuItem.Enabled = btnUninstall.Visible = false;
          break;
        case WrapperInstalledState.NotInstalled:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusInfoColor;
          installMenuItem.Enabled = btnInstall.Visible = true;
          uninstallMenuItem.Enabled = btnUninstall.Visible = false;
          break;
        case WrapperInstalledState.RdpWrap:
          lblWrapperStateValue.Text = "RdpWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusOkColor;
          string wrapperIniPath = null;
          if (!string.IsNullOrEmpty(wrapper.WrapperPath)) {
            var wrappedDir = Path.GetDirectoryName(wrapper.WrapperPath);
            if (wrappedDir != null) {
              wrapperIniPath = Path.Combine(wrappedDir, Wrapper.RdpWrapIniName);
              checkSupported = File.Exists(wrapperIniPath);
            }
          }
          if (wrapperIniPath != wrapperIniLastPath) {
            wrapperIniLastPath = wrapperIniPath;
            wrapperIniLastChecked = DateTime.MinValue;
          }
          lblWrapperVersion.Visible = true;
          installMenuItem.Enabled = btnInstall.Visible = false;
          uninstallMenuItem.Enabled = btnUninstall.Visible = true;
          break;
        case WrapperInstalledState.ThirdParty:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusErrorColor;
          installMenuItem.Enabled = btnInstall.Visible = false;
          uninstallMenuItem.Enabled = btnUninstall.Visible = false;
          break;
        case WrapperInstalledState.TermWrap:
          lblWrapperStateValue.Text = "TermWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusOkColor;
          checkSupported = null;
          wrapperIniLastChecked = DateTime.MinValue;
          wrapperIniLastPath = null;
          lblWrapperVersion.Visible = false;
          installMenuItem.Enabled = btnInstall.Visible = false;
          uninstallMenuItem.Enabled = btnUninstall.Visible = true;
          break;
      }

      switch (wrapper.GetServiceState()) {
        case ServiceControllerStatus.Stopped:
          lblServiceStateValue.Text = "Stopped";
          lblServiceStateValue.ForeColor = Theme.Current.StatusErrorColor;
          break;
        case ServiceControllerStatus.StartPending:
          lblServiceStateValue.Text = "Starting..";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.StopPending:
          lblServiceStateValue.Text = "Stopping...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.Running:
          lblServiceStateValue.Text = "Running";
          lblServiceStateValue.ForeColor = Theme.Current.StatusOkColor;
          break;
        case ServiceControllerStatus.ContinuePending:
          lblServiceStateValue.Text = "Resuming...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.PausePending:
          lblServiceStateValue.Text = "Suspending...";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        case ServiceControllerStatus.Paused:
          lblServiceStateValue.Text = "Suspended";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
        default:
          lblServiceStateValue.Text = "Unknown";
          lblServiceStateValue.ForeColor = Theme.Current.ForegroundColor;
          break;
      }

      if (WinStationHelper.IsListenerWorking()){
        lblListenerStateValue.Text = "Listening";
        lblListenerStateValue.ForeColor = Theme.Current.StatusOkColor;
      }
      else {
        lblListenerStateValue.Text = "Not listening";
        lblListenerStateValue.ForeColor = Theme.Current.StatusErrorColor;
      }

      if (string.IsNullOrEmpty(wrapper.WrapperPath) || !File.Exists(wrapper.WrapperPath)) {
        lblWrapperVersion.Text = "N/A";
        lblWrapperVersion.ForeColor = Theme.Current.StatusErrorColor;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(wrapper.WrapperPath);
        lblWrapperVersion.Text = Wrapper.GetVersionString(versionInfo);
        lblWrapperVersion.ForeColor = Theme.Current.ForegroundColor;
      }

      if (!File.Exists(wrapper.TermSrvFile)) {
        txtServiceVersion.Text = "N/A";
        txtServiceVersion.ForeColor = Theme.Current.StatusErrorColor;
      }
      else {
        var versionInfo = FileVersionInfo.GetVersionInfo(wrapper.TermSrvFile);
        txtServiceVersion.Text = Wrapper.GetVersionString(versionInfo);
        txtServiceVersion.ForeColor = Theme.Current.ForegroundColor;

#if LITEVERSION
        btnGenerate.Visible = false;
#else
        generateMenuItem.Enabled = btnGenerate.Visible = wrapperInstalled == WrapperInstalledState.RdpWrap;
#endif
        lblSupported.Visible = checkSupported is true;
        if (checkSupported is true) {
          if (versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 0 ||
              versionInfo.FileMajorPart == 6 && versionInfo.FileMinorPart == 1) {
            lblSupported.Text = "[supported partially]";
            lblSupported.ForeColor = Theme.Current.StatusInfoColor;
          }
          else {
            var lastModified = File.GetLastWriteTime(wrapperIniLastPath);
            if (lastModified > wrapperIniLastChecked) {
              var iniContent = File.ReadAllText(wrapperIniLastPath);
              wrapperIniLastSupported = iniContent.Contains("[" + Wrapper.GetVersionString(versionInfo) + "]");
              wrapperIniLastChecked = lastModified;
            }
            if (wrapperIniLastSupported) {
              lblSupported.Text = "[fully supported]";
              lblSupported.ForeColor = Theme.Current.StatusOkColor;
              return;
            }
          }
          lblSupported.Text = "[not supported]";
          lblSupported.ForeColor = Theme.Current.StatusErrorColor;
        }
        else if (!checkSupported.HasValue) {
          lblSupported.Text = "[supported]";
          lblSupported.ForeColor = Theme.Current.StatusOkColor;
        }
      }
    }

    private void btnTest_Click(object sender, EventArgs e) {
      Process.Start("mstsc.exe", $"/v:127.0.0.2:{oldPort}");
    }

    private void btnGenerate_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
#if LITEVERSION
        MessageBox.Show("No need for Ini file with TermWrap.", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#else
        wrapper.GenerateIniFile(wrapperIniLastPath, true, (message) =>
          MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        );
#endif
      }
      finally {
        SetControlsState(true);
      }
    }

    private void AddToLog(string message, Logger.StateKind state, bool newLine) {
      if (InvokeRequired) {
        Invoke(new Action<string, Logger.StateKind, bool>(AddToLog), message, state);
        return;
      }

      if (newLine)
        txtLog.AppendLine($"{DateTime.Now:T} - ", Theme.Current.ForegroundColor);
      switch (state) {
        case Logger.StateKind.Error:
          txtLog.AppendLine(message, Theme.Current.StatusErrorColor, false);
          break;
        case Logger.StateKind.Info:
          txtLog.AppendLine(message, Theme.Current.StatusOkColor, false);
          break;
        default:
          txtLog.AppendLine(message, Theme.Current.ForegroundColor, false);
          break;
      }

      // if (!string.IsNullOrEmpty(_logFileName))
      //   File.AppendAllText(_logFileName, $"{DateTime.Now:T} - {message}\n");
      txtLog.ScrollToCaret();
      Application.DoEvents();
    }

    private void btnInstall_Click(object sender, EventArgs e) {

      try {
        SetControlsState(false);
#if LITEVERSION
        wrapper.Install();
#else
        var answer = MessageBox.Show("Choose:\n'Yes' - to install 'TermWrap'\n'No' - to install 'RdpWrap'\n'Cancel' - if you are not a confident person", Updater.ApplicationTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        switch (answer) {
          case DialogResult.Yes:
            wrapper.Install(true);
            break;
          case DialogResult.No:
            wrapper.Install(false);
            break;
          default:
            return;
        }
#endif
        //cbxAllowTSConnections.Checked = true;
        //btnApply.PerformClick();
        //btnRestartService.PerformClick();
      }
      catch (Exception ex) {
        var message = "Failed to Install: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void btnUninstall_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        wrapper.Uninstall();
        
        //cbxAllowTSConnections.Checked = false;
        //btnApply.PerformClick();
      }
      catch (Exception ex) {
        var message = "Failed to Install: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void SetControlsState(bool enabled) {
      refreshTimer.Enabled = enabled;
      btnInstall.Enabled = installMenuItem.Enabled = enabled;
      btnUninstall.Enabled = uninstallMenuItem.Enabled = enabled;
      btnRestartService.Enabled = restartServiceMenuItem.Enabled = enabled;
      btnGenerate.Enabled = generateMenuItem.Enabled = enabled;
    }
  }
}
