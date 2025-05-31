using NetFwTypeLib;
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
    private SupportedWrappers preferredWrapper;
    private UserOption showAntivirusWarn;
    private UserOption addDefenderExclusion;
    private UserOption setFirewallRule;

    public MainForm() {

      InitializeComponent();

      Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
      var appArch = Environment.Is64BitProcess ? "x64" : "x86";
      var sysArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
      var title = $"{Updater.ApplicationTitle} v{Updater.CurrentVersion} {appArch}";
      if (appArch != sysArch)
        title += "/" + sysArch;
      Text = title;

      settings = new PersistentSettings();
      settings.Load();
      InitializeTheme();

      var sizeSpan = Height - ClientSize.Height;
      MinimumSize = new Size { Width = Width, Height = sizeSpan + gbxGeneralSettings.Height + gbxStatus.Height + mainMenu.Height };

      var showLog = new UserOption("showLog", true, showLogToolStripMenuItem, settings);
      showLog.Changed += delegate {
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
      var portable = new UserOption("portable", true, storeSeiingsInFileToolStripMenuItem, settings);
      portable.Changed += delegate {
        settings.IsPortable = portable.Value;
      };
      showAntivirusWarn = new UserOption("showAntivirusWarn", true, showAntivirusWarnMenuItem, settings);
      addDefenderExclusion = new UserOption("addDefenderExclusion", true, addDefenderExclusionMenuItem, settings);
      setFirewallRule = new UserOption("setFirewallRule", true, addFirewallRuleMenuItem, settings);

      if (!Enum.TryParse(settings.GetValue("preferredWrapper", "TermWrap"), out preferredWrapper)){
        preferredWrapper = SupportedWrappers.TermWrap;
      }
      foreach (SupportedWrappers wrap in Enum.GetValues(typeof(SupportedWrappers))) {
        var menuItem = new ToolStripRadioButtonMenuItem(wrap.ToString(), null, (sender, e) => {
          if (sender is not ToolStripRadioButtonMenuItem menuItem || !Enum.TryParse(menuItem.Text, out preferredWrapper))
            return;
          settings.SetValue("preferredWrapper", preferredWrapper.ToString());
          menuItem.Checked = true;
        });
        wrapperToInstallMenuItem.DropDownItems.Add(menuItem);
        if (wrap == preferredWrapper)
          menuItem.Checked = true;
      }

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

      refreshTimer = new Timer();
      refreshTimer.Tick += TimerTick;
      refreshTimer.Interval = 1000;

      Load += (sender, e) => { 
        RefreshSystemSettings();
        TimerTick(null, EventArgs.Empty);
        refreshTimer.Enabled = true;

        cbxSingleSessionPerUser.CheckedChanged += (s, e) => { wrapper.SingleSessionPerUser = cbxSingleSessionPerUser.Checked; };
        cbxAllowTSConnections.CheckedChanged += (s, e) => { wrapper.AllowTsConnections = cbxAllowTSConnections.Checked; };
        cbDontDisplayLastUser.CheckedChanged += (s, e) => { wrapper.DontDisplayLastUser = cbDontDisplayLastUser.Checked; };
        rgShadowOptions.SelectedIndexChanged += (s, e) => { wrapper.ShadowOptions = rgShadowOptions.SelectedIndex; };
        cbxHonorLegacy.CheckedChanged += (s, e) => { wrapper.HonorLegacy = cbxHonorLegacy.Checked; };
        cbxAllowPlaybackRedirect.CheckedChanged += (s, e) => { wrapper.AllowHostPlaybackRedirect = cbxAllowPlaybackRedirect.Checked; };
        cbxAllowAudioCapture.CheckedChanged += (s, e) => { wrapper.AllowClientAudioCapture = cbxAllowAudioCapture.Checked; };
        cbxAllowVideoCapture.CheckedChanged += (s, e) => { wrapper.AllowClientVideoCapture = cbxAllowVideoCapture.Checked; };
        cbxAllowPnp.CheckedChanged += (s, e) => { wrapper.AllowPnpRedirect = cbxAllowPnp.Checked; };

        numRDPPort.ValueChanged += (s, e) => {
          var newPort = (int)numRDPPort.Value;
          if (oldPort != newPort) {
            if (setFirewallRule.Value) {
              try {
                var tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                var fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
                var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                INetFwRule inboundRule = null;
                try {
                  inboundRule = firewallPolicy.Rules.Item(Updater.ApplicationName);
                }
                catch (FileNotFoundException) {
                  //ignore
                  //if (inboundRule == null) {
                  //  foreach (INetFwRule2 rule in firewallPolicy.Rules) {
                  //    if (rule.Name == Updater.ApplicationName) {
                  //      inboundRule = rule;
                  //      break;
                  //    }
                  //  }
                  //}
                }
                var createNewRule = inboundRule != null;
                if (inboundRule == null) {
                  inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                  inboundRule.Name = Updater.ApplicationName;
                }

                inboundRule.Enabled = true;
                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                inboundRule.Protocol = 6; // TCP
                inboundRule.LocalPorts = newPort.ToString();
                inboundRule.Profiles = fwPolicy2.CurrentProfileTypes;

                if (!createNewRule) {
                  firewallPolicy.Rules.Add(inboundRule);
                  logger.Log($"Firewall rule added for port {newPort}", Logger.StateKind.Info);
                }
                else {
                  logger.Log($"Firewall rule updated to use port {newPort}", Logger.StateKind.Info);
                }
              }
              catch (Exception ex) {
                logger.Log($"Error configuring firewall: " + ex.Message, Logger.StateKind.Error);
              }
            }
            oldPort = wrapper.RdpPort = newPort;
          }
        };
        numMaxConnections.ValueChanged += (s, e) => {
          wrapper.MaximumConnectionsAllowed = (int)numMaxConnections.Value;
        };

        rgNLAOptions.SelectedIndexChanged += (s, e) => {
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
        };
      };

      Updater.Subscribe(
        (message, isError) => {
          MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OK, isError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        },
        (message) => {
          return MessageBox.Show(message, Updater.ApplicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
        },
        Application.Exit
      );

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
        numRDPPort.Value = oldPort = wrapper.RdpPort;
        numMaxConnections.Enabled = OperatingSystemHelper.IsWindowsServer;
        numMaxConnections.Value = wrapper.MaximumConnectionsAllowed;
        
        rgNLAOptions.SelectedIndex = wrapper.SecurityLayer switch {
          0 when wrapper.UserAuthentication == 0 => 0,
          1 when wrapper.UserAuthentication == 0 => 1,
          2 when wrapper.UserAuthentication == 1 => 2,
          _ => rgNLAOptions.SelectedIndex
        };

        rgShadowOptions.SelectedIndex = wrapper.ShadowOptions;
        cbDontDisplayLastUser.Checked = wrapper.DontDisplayLastUser;
        cbxAllowPlaybackRedirect.Checked = wrapper.AllowHostPlaybackRedirect;
        cbxAllowAudioCapture.Checked = wrapper.AllowClientAudioCapture;
        cbxAllowVideoCapture.Checked = wrapper.AllowClientVideoCapture;
        cbxAllowPnp.Checked = wrapper.AllowPnpRedirect;

        logger.Log(" Done", Logger.StateKind.Info, false);
      }
      catch (Exception ex) {
        var message = "Error loading settings: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        //MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnClose_Click(object sender, EventArgs e) {
      Close();
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

      var allThemes = CustomTheme.GetAllThemes("themes", "rdpWrapper.themes").OrderBy(x => x.DisplayName).ToList();

      var setTheme = allThemes.FirstOrDefault(theme => settings.GetValue("theme", "auto") == theme.Id);
      if (setTheme != null) {
        Theme.Current = setTheme;
      }

      AddThemeMenuItems(allThemes.Where(t => t is not CustomTheme));
      var customThemes = allThemes.Where(t => t is CustomTheme).ToList();
      if (customThemes.Count > 0) {
        themeMenuItem.DropDownItems.Add("-");
        AddThemeMenuItems(customThemes);
      }

      if (setTheme == null && themeMenuItem.DropDownItems.Count > 0)
        themeMenuItem.DropDownItems[0].PerformClick();

      Theme.Current.Apply(this);
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
          uninstallMenuItem.Enabled = installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = false;
          break;
        case WrapperInstalledState.NotInstalled:
          lblWrapperStateValue.Text = "Not installed";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusInfoColor;
          installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          uninstallMenuItem.Enabled = false;
          btnInstall.Text = "Install";
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
          installMenuItem.Enabled = false;
          uninstallMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          btnInstall.Text = "Uninstall";
          break;
        case WrapperInstalledState.ThirdParty:
          lblWrapperStateValue.Text = "3rd-party";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusErrorColor;
          uninstallMenuItem.Enabled = installMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = false;
          break;
        case WrapperInstalledState.TermWrap:
          lblWrapperStateValue.Text = "TermWrap";
          lblWrapperStateValue.ForeColor = Theme.Current.StatusOkColor;
          checkSupported = null;
          wrapperIniLastChecked = DateTime.MinValue;
          wrapperIniLastPath = null;
          installMenuItem.Enabled = false;
          uninstallMenuItem.Enabled = btnInstall.Enabled = btnInstall.Visible = true;
          btnInstall.Text = "Uninstall";
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
        generateMenuItem.Enabled = btnGenerate.Enabled = btnGenerate.Visible = wrapperInstalled == WrapperInstalledState.RdpWrap;
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

    private void addUserToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        SetControlsState(false);
        addUserToolStripMenuItem.Enabled = false;
        if (InputForm.GetValue(Updater.ApplicationName, "Please enter the 'User name':", out var userName) == DialogResult.OK) {
          using (var usersManager = new LocalUsersManager(logger)) {
            if (usersManager.GetRemoteDesktopUsers().Any(u => u.Equals(userName, StringComparison.OrdinalIgnoreCase))) {
              MessageBox.Show($"User '{userName}' is already a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
              var user = usersManager.CreateUserIfNotExist(userName);
              //if (user.GetAuthorizationGroups().Any(g => g.Sid.Value == LocalUsersManager.RemoteDesktopUsersGroupSid)) {
              //  MessageBox.Show($"User '{userName}' is already a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
              //}
              //else {
                if (InputForm.GetValue(Updater.ApplicationName, "Please enter the 'Password':", out var password) == DialogResult.OK) {
                  usersManager.SetUserPassword(user, password);
                  usersManager.EnsureUserInRemoteDesktopUsers(user);
                  MessageBox.Show($"User '{user.Name}' is a member of 'Remote Desktop Users' group.", Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
              //}
            }
          }
        }
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
        MessageBox.Show(ex.Message, Updater.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
      finally {
        SetControlsState(true);
        addUserToolStripMenuItem.Enabled = true;
      }
    }

    private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e) {
      Process.Start("lusrmgr.msc");
    }

    private void manageUsersoldToolStripMenuItem_Click(object sender, EventArgs e) {
      Process.Start("netplwiz");
      //Process.Start("control", "userpasswords2");
    }

    private void btnTest_Click(object sender, EventArgs e) {

      var width = Screen.PrimaryScreen.Bounds.Width * 2 / 3;
      var height = Screen.PrimaryScreen.Bounds.Height * 2 / 3;
      var arguments = $"/v:127.0.0.2:{oldPort} /w:{width} /h:{height}";
      if (MessageBox.Show("Test as public connection? (Passwords and bitmaps aren't cached)", Updater.ApplicationTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
        arguments += " /public";
      }
      //https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/mstsc
      Process.Start("mstsc.exe", arguments);
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
      
      var operation = btnInstall.Text;
      SetControlsState(false);
      try {
        if (operation == "Install") {
          wrapper.Install(preferredWrapper, addDefenderExclusion.Value);
          if (showAntivirusWarn.Value) {
            MessageBox.Show($"Wrapper was installed in folder: '{wrapper.WrapperFolderPath}'\nYour antivirus software might block or interfere with wrapper folder.\nPlease verify that it is included in the exclusion list!", Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
        else {
          wrapper.Uninstall();
        }
      }
      catch (Exception ex) {
        var message = $"Failed to {operation}: {ex.Message}";
        logger.Log(message, Logger.StateKind.Error);
        MessageBox.Show(message, Updater.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        SetControlsState(true);
      }
    }

    private void SetControlsState(bool enabled) {
      refreshTimer.Enabled = enabled;
      if (!enabled) {
        btnInstall.Enabled = installMenuItem.Enabled = uninstallMenuItem.Enabled = enabled;
        btnGenerate.Enabled = generateMenuItem.Enabled = enabled;
      }
      btnRestartService.Enabled = restartServiceMenuItem.Enabled = enabled;
    }
  }
}
