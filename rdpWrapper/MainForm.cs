using sergiye.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace rdpWrapper {

  internal partial class MainForm : Form {

    private const int MfSeparator = 0x00000800;
    private const int MfByPosition = 0x400;
    private const int MfWmSysCommand = 0x112;
    private const int MfSysMenuCheckUpdates = 1000;
    private const int MfSysMenuAboutId = 1001;

    private int oldPort;
    private string wrapperIniLastPath;
    private DateTime wrapperIniLastChecked = DateTime.MinValue;
    private bool wrapperIniLastSupported;
    private readonly Timer refreshTimer;
    private readonly Logger logger;
    private readonly Wrapper wrapper;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      var title =$"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {(Environment.Is64BitProcess ? "x64" : "x86")}";
      Text = title;

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

      var menuHandle = GetSystemMenu(Handle, false); // Note: to restore default set true
      InsertMenu(menuHandle, 5, MfByPosition | MfSeparator, 0, string.Empty); // <-- Add a menu separator
      InsertMenu(menuHandle, 6, MfByPosition, MfSysMenuCheckUpdates, "Check for new version");
      InsertMenu(menuHandle, 7, MfByPosition, MfSysMenuAboutId, "&About…");

      Load += (sender, e) => { RefreshSystemSettings(); };

      Theme.SetAutoTheme();
      Theme.Current.Apply(this);
      btnTheme.Text = Theme.Current.DisplayName;

      refreshTimer = new Timer();
      refreshTimer.Tick += TimerTick;
      refreshTimer.Interval = 1000;

      var timer = new Timer();
      timer.Tick += async (_, _) => {
        timer.Enabled = false;
        timer.Enabled = !await Updater.CheckForUpdatesAsync(true).ConfigureAwait(false);
      };
      timer.Interval = 3000;
      timer.Enabled = true;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll")]
    private static extern bool InsertMenu(IntPtr hMenu, int wPosition, int wFlags, int wIdNewItem, string lpNewItem);
    
    protected override void WndProc(ref Message m) {

      base.WndProc(ref m);
      
      if (m.Msg != MfWmSysCommand)
        return;
      
      switch ((int)m.WParam) {
        case MfSysMenuAboutId:
          var asm = GetType().Assembly;
          if (MessageBox.Show($"{Updater.ApplicationTitle} {asm.GetName().Version.ToString(3)} {(Environment.Is64BitProcess ? "x64" : "x32")}\nWritten by Sergiy Egoshyn (egoshin.sergey@gmail.com)\nDo you want to know more?", Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
            Process.Start($"https://github.com/{Updater.ApplicationCompany}/{Updater.ApplicationName}");
          }
          break;
        case MfSysMenuCheckUpdates:
          Updater.CheckForUpdates(false);
          break;
      }
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

    private void btnTheme_Click(object sender, EventArgs e) {

      if (Theme.Current.DisplayName == "Light") {
        Theme.Current = new DarkTheme();
      }
      else {
        Theme.Current = new LightTheme();
      }
      btnTheme.Text = Theme.Current.DisplayName;
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
          btnInstall.Visible = false;
          btnUninstall.Visible = false;
          break;
        case WrapperInstalledState.NotInstalled:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusInfoColor;
          btnInstall.Visible = true;
          btnUninstall.Visible = false;
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
          btnInstall.Visible = false;
          btnUninstall.Visible = true;
          break;
        case WrapperInstalledState.ThirdParty:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusErrorColor;
          btnInstall.Visible = false;
          btnUninstall.Visible = false;
          break;
        case WrapperInstalledState.TermWrap:
          lblWrapperStateValue.Text = "TermWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusOkColor;
          checkSupported = null;
          wrapperIniLastChecked = DateTime.MinValue;
          wrapperIniLastPath = null;
          lblWrapperVersion.Visible = false;
          btnInstall.Visible = false;
          btnUninstall.Visible = true;
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

        btnGenerate.Visible = wrapperInstalled == WrapperInstalledState.RdpWrap;
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
#if LIGHTVERSION
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
#if LIGHTVERSION
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
      btnUninstall.Enabled = enabled;
      btnRestartService.Enabled = enabled;
      btnGenerate.Enabled = enabled;
      btnInstall.Enabled = enabled;
    }
  }
}
