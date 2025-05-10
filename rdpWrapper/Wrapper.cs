using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using Microsoft.Win32;
using sergiye.Common;

namespace rdpWrapper {

  internal enum WrapperInstalledState {
    Unknown,
    NotInstalled,
    ThirdParty,
    RdpWrap,
    TermWrap,
  }

  internal class Wrapper {

    private const string RegKey = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
    private const string RegTermServiceKey = @"SYSTEM\CurrentControlSet\Services\TermService";
    private const string RegRdpKey = @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp";
    private const string RegWinLogonKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
    private const string RegTsKey = @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services";

    private const string ValueSingleSession = "fSingleSessionPerUser";
    private const string ValueDenyTsConnections = "fDenyTSConnections";
    private const string ValuePort = "PortNumber";
    private const string ValueDontDisplayLastUserName = "DontDisplayLastUsername";
    private const string ValueHonorLegacy = "HonorLegacySettings";
    private const string ValueNla = "UserAuthentication";
    private const string ValueSecurity = "SecurityLayer";
    private const string ValueShadow = "Shadow";
    
    private const string RdpServiceName = "TermService";
    
    internal const string RdpWrapIniName = "rdpwrap.ini";
    private const string RdpWrapDllName = "rdpwrap.dll";
    private const string TermSrvName = "termsrv.dll";
    private const string TermWrapDllName = "TermWrap.dll";
    private const string UmWrapDllName = "UmWrap.dll";
    private const string ZydisDllName = "Zydis.dll";

    private readonly Logger logger;
    private readonly ServiceHelper serviceHelper;
    private readonly RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

    internal Wrapper(Logger logger) {
      this.logger = logger;
      serviceHelper = new ServiceHelper(logger);

      WrapperFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "RDP Wrapper");

      if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess) {
        TermSrvFile = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative"), TermSrvName);
      }
      else {
        TermSrvFile = Path.Combine(Environment.SystemDirectory, TermSrvName);
      }

      ReadSettings();
    }

    internal bool SingleSessionPerUser { get; set; }
    internal bool AllowTsConnections { get; set; }
    internal bool HonorLegacy { get; set; }
    internal int RdpPort { get; set; }
    internal int UserAuthentication { get; set; }
    internal int SecurityLayer { get; set; }
    internal int ShadowOptions { get; set; }
    internal bool DontDisplayLastUser { get; set; }

    internal string WrapperPath { get; private set; }

    internal readonly string TermSrvFile;
    internal readonly string WrapperFolderPath;

    private void ReadSettings() {
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)) {
        using (var key = baseKey.OpenSubKey(RegKey)) {
          if (key != null) {
            SingleSessionPerUser = Convert.ToInt32(key.GetValue(ValueSingleSession, 0)) != 0;
            AllowTsConnections = Convert.ToInt32(key.GetValue(ValueDenyTsConnections, 0)) == 0;
            HonorLegacy = Convert.ToInt32(key.GetValue(ValueHonorLegacy, 0)) != 0;
          }
        }

        using (var key = baseKey.OpenSubKey(RegRdpKey)) {
          if (key != null) {
            RdpPort = Convert.ToInt32(key.GetValue(ValuePort, 3389));
            UserAuthentication = Convert.ToInt32(key.GetValue(ValueNla, 0));
            SecurityLayer = Convert.ToInt32(key.GetValue(ValueSecurity, 0));
            ShadowOptions = Convert.ToInt32(key.GetValue(ValueShadow, 0));
          }
        }

        using (var key = baseKey.OpenSubKey(RegWinLogonKey)) {
          if (key != null)
            DontDisplayLastUser = Convert.ToInt32(key.GetValue(ValueDontDisplayLastUserName, 0)) != 0;
        }
      }
    }

    internal void SaveSettings() {

      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)) {
        using (var key = baseKey.OpenSubKey(RegKey, writable: true)) {
          if (key != null) {
            key.SetValue(ValueSingleSession, SingleSessionPerUser ? 1 : 0, RegistryValueKind.DWord);
            key.SetValue(ValueDenyTsConnections, AllowTsConnections ? 0 : 1, RegistryValueKind.DWord);
            key.SetValue(ValueHonorLegacy, HonorLegacy ? 1 : 0, RegistryValueKind.DWord);
          }
        }

        using (var key = baseKey.OpenSubKey(RegRdpKey, true)) {
          if (key != null) {
            key.SetValue(ValuePort, RdpPort, RegistryValueKind.DWord);
            key.SetValue(ValueNla, UserAuthentication, RegistryValueKind.DWord);
            key.SetValue(ValueSecurity, SecurityLayer, RegistryValueKind.DWord);
            key.SetValue(ValueShadow, ShadowOptions, RegistryValueKind.DWord);
          }
        }

        using (var key = baseKey.CreateSubKey(RegTsKey)) {
          key?.SetValue(ValueShadow, ShadowOptions, RegistryValueKind.DWord);
        }

        using (var key = baseKey.CreateSubKey(RegWinLogonKey)) {
          key?.SetValue(ValueDontDisplayLastUserName, DontDisplayLastUser ? 1 : 0, RegistryValueKind.DWord);
        }
      }
    }
 
    internal WrapperInstalledState CheckWrapperInstalled() {
      WrapperPath = string.Empty;
      try {
        using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)) {
          using (var serviceKey = baseKey.OpenSubKey(RegTermServiceKey)) {
            if (serviceKey == null)
              return WrapperInstalledState.Unknown;
            var termServiceHost = serviceKey.GetValue("ImagePath") as string;
            if (string.IsNullOrWhiteSpace(termServiceHost) || !termServiceHost.ToLower().Contains("svchost.exe"))
              return WrapperInstalledState.ThirdParty;
          }

          string termServicePath;
          using (var paramKey = baseKey.OpenSubKey(RegTermServiceKey + "\\Parameters")) {
            if (paramKey == null)
              return WrapperInstalledState.Unknown;

            termServicePath = paramKey.GetValue("ServiceDll") as string;
            if (string.IsNullOrWhiteSpace(termServicePath))
              return WrapperInstalledState.Unknown;
          }

          //if (Environment.Is64BitProcess || !Environment.Is64BitOperatingSystem) {
          //  if (!File.Exists(termServicePath))
          //    return WrapperInstalledState.Unknown;
          //}
          var wrapperName = Path.GetFileName(termServicePath);
          if (TermSrvName.Equals(wrapperName, StringComparison.OrdinalIgnoreCase))
            return WrapperInstalledState.NotInstalled;
          if (RdpWrapDllName.Equals(wrapperName, StringComparison.OrdinalIgnoreCase)) {
            WrapperPath = termServicePath;
            //WrapperFolderPath = Path.GetDirectoryName(termServicePath);
            return WrapperInstalledState.RdpWrap;
          }
          if (TermWrapDllName.Equals(wrapperName, StringComparison.OrdinalIgnoreCase)) {
            WrapperPath = termServicePath;
            return WrapperInstalledState.TermWrap;
          }
        }
        return WrapperInstalledState.ThirdParty;
      }
      catch {
        return WrapperInstalledState.Unknown;
      }
    }
 
    internal static string GetVersionString(FileVersionInfo versionInfo) {
      return versionInfo.ProductMajorPart + "." + versionInfo.ProductMinorPart + "." + versionInfo.ProductBuildPart + "." + versionInfo.ProductPrivatePart;
    }

    #region Service wrapper
    
    internal void StopService(TimeSpan timeout) {
      serviceHelper.StopService(RdpServiceName, timeout);
    }
    
    internal void StartService(TimeSpan timeout) {
      serviceHelper.StartService(RdpServiceName, timeout);
    }
    
    internal ServiceControllerStatus? GetServiceState() {
      return serviceHelper.GetServiceState(RdpServiceName);
    }

    #endregion

#if !LITEVERSION
    internal void GenerateIniFile(string destFilePath, bool executeCleanup = true, Action<string> onError = null) {

      string iniFile = null;
      string offsetFinder = null;
      string zydis = null;
      try {
        logger.Log("Generating config...");
        var workingDir = Path.GetTempPath();
        iniFile = ExtractResourceFile(RdpWrapIniName, workingDir, true);
        offsetFinder = ExtractResourceFile("RDPWrapOffsetFinder.exe", workingDir, archPrefix: true);
        zydis = ExtractResourceFile(ZydisDllName, workingDir, archPrefix: true);
        var p = StartProcess("cmd", $"/c \"{offsetFinder}\" >> {RdpWrapIniName} & exit", workingDir);
        p.WaitForExit();
        if (p.ExitCode == 0)
          logger.Log(" Done", Logger.StateKind.Info, false);
        else
          logger.Log(" Error", Logger.StateKind.Error, false);
      }
      catch (Exception ex) {
        var message = "Failed to generate config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        onError?.Invoke(message);
      }
      finally {
        if (executeCleanup) {
          SafeDeleteFile(offsetFinder);
          SafeDeleteFile(zydis);
        }
      }

      if (string.IsNullOrEmpty(iniFile) || !File.Exists(iniFile)) return;
      
      try {
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));

        SafeDeleteFile(destFilePath);
        File.Move(iniFile, destFilePath);

        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
      }
      catch (Exception ex) {
        var message = "Failed to update config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        onError?.Invoke(message);
      }
    }
#endif

#if LITEVERSION
    internal void Install() {
      const bool useTermWrap = true;
#else
    internal void Install(bool useTermWrap) {
#endif
      //Uninstall();
      var prevServiceState = serviceHelper.GetServiceState(RdpServiceName);
      if (prevServiceState is ServiceControllerStatus.Running)
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));

      Directory.CreateDirectory(WrapperFolderPath);
      logger.Log("Folder created: " + WrapperFolderPath);

      AddDirectorySecurity(WrapperFolderPath, [
        "S-1-5-18", // Local System account
        "S-1-5-6", // Service group
        "S-1-5-32-545", // SID for "Users"
      ] , FileSystemRights.FullControl, AccessControlType.Allow);

      string wrapPath;
      if (useTermWrap) {
        wrapPath = ExtractResourceFile(TermWrapDllName, WrapperFolderPath, archPrefix: true);
        logger.Log("Extracted TermWrap.dll -> " + wrapPath);
        var zydis = ExtractResourceFile(ZydisDllName, WrapperFolderPath, archPrefix: true);
        logger.Log("Extracted zydis.dll -> " + zydis);
        if (Environment.Is64BitProcess) {
          var umWrap = ExtractResourceFile(UmWrapDllName, WrapperFolderPath, archPrefix: true);
          logger.Log("Extracted umWrap.dll -> " + umWrap);
        }
      }
#if !LITEVERSION
      else {
        wrapPath = ExtractResourceFile(RdpWrapDllName, WrapperFolderPath, archPrefix: true);
        logger.Log("Extracted rdpWrap.dll -> " + wrapPath);
      }
#endif

      logger.Log("Configuring service library...");
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)) {
        using (var reg = baseKey.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true)) {
          if (reg == null) {
            logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
            return;
          }
          reg.SetValue("ServiceDll", wrapPath, RegistryValueKind.ExpandString);
        }
      }
#if !LITEVERSION
      if (!useTermWrap) {
        GenerateIniFile(Path.Combine(WrapperFolderPath, RdpWrapIniName));
      }
#endif
      logger.Log(" Done", Logger.StateKind.Info, false);
      if (prevServiceState is ServiceControllerStatus.Running)
        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
    }

    internal void Uninstall() {
      logger.Log("Resetting service library...");
      using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView)) {
        using (var reg = baseKey.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true)) {
          if (reg == null) {
            logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
            return;
          }
          reg.SetValue("ServiceDll", @"%SystemRoot%\System32\termsrv.dll", RegistryValueKind.ExpandString);
        }
      }
      logger.Log(" Done", Logger.StateKind.Info, false);
      var serviceState = serviceHelper.GetServiceState(RdpServiceName); 
      if (serviceState is ServiceControllerStatus.Running) {
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));
      }

      logger.Log("Removed folder: " + WrapperFolderPath);
      try {
        Directory.Delete(WrapperFolderPath, true);
      }
      catch (DirectoryNotFoundException) {
        //ignore
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error); //todo: ignore?
      }
      if (serviceState.HasValue)
        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
    }
    
    #region supplementary methods
    
    private string ExtractResourceFile(string resourceName, string path, bool deleteExisting = false, bool archPrefix = false) {
      var filePath = Path.Combine(path, resourceName);
      if (File.Exists(filePath)) {
        if (!deleteExisting) {
          return filePath; //todo: delete
        }
        SafeDeleteFile(filePath);
      }
      try {
        var type = GetType();
        
        var scriptsPath = archPrefix 
          ? $"{type.Namespace}.externals.{(Environment.Is64BitProcess ? "x64" : "x86")}.{resourceName}"
          : $"{type.Namespace}.externals.{resourceName}";
        using var stream = type.Assembly.GetManifestResourceStream(scriptsPath);
        using var fileStream = File.Create(filePath);
        stream?.Seek(0, SeekOrigin.Begin);
        stream?.CopyTo(fileStream);
        return filePath;
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
        return null;
      }
    }

    private void SafeDeleteFile(string filePath) {
      try {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) File.Delete(filePath);
      }
      catch (Exception ex) {
        logger.Log(ex.Message, Logger.StateKind.Error);
      }
    }

    internal static Process StartProcess(string app, string arg, string workingDir = null) {

      var p = new Process();
      p.StartInfo.UseShellExecute = true;
      p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      p.StartInfo.FileName = app;
      p.StartInfo.WorkingDirectory = (workingDir ?? Path.GetDirectoryName(app)) ?? string.Empty;
      p.StartInfo.Arguments = arg;
      p.StartInfo.RedirectStandardOutput = false;
      p.StartInfo.RedirectStandardInput = false;
      p.StartInfo.RedirectStandardError = false;
      p.Start();
      return p;
    }
 
    private static void AddDirectorySecurity(string fileName, string[] sids, FileSystemRights rights, AccessControlType controlType) {
      var dInfo = new DirectoryInfo(fileName);
      var dSecurity = dInfo.GetAccessControl();
      foreach(var sid in sids) {
        var sIdentifier = new SecurityIdentifier(sid);
        dSecurity.AddAccessRule(new FileSystemAccessRule(sIdentifier, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, controlType));
      }
      dInfo.SetAccessControl(dSecurity);
    }
    
    #endregion
  }
}