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

    private readonly Logger logger;
    private readonly ServiceHelper serviceHelper;

    internal Wrapper(Logger logger) {
      this.logger = logger;
      serviceHelper = new ServiceHelper(logger);
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

    internal readonly string TermSrvFile = Path.Combine(Environment.SystemDirectory, TermSrvName);
    internal readonly string WrapperFolderPath = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), "RDP Wrapper"); // programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

    public event Action<string> OnError;

    private void ReadSettings() {

      using (var key = Registry.LocalMachine.OpenSubKey(RegKey)) {
        if (key != null) {
          SingleSessionPerUser = Convert.ToInt32(key.GetValue(ValueSingleSession, 0)) != 0;
          AllowTsConnections = Convert.ToInt32(key.GetValue(ValueDenyTsConnections, 0)) == 0;
          HonorLegacy = Convert.ToInt32(key.GetValue(ValueHonorLegacy, 0)) != 0;
        }
      }
      
      using (var key = Registry.LocalMachine.OpenSubKey(RegRdpKey)) {
        if (key != null) {
          RdpPort = Convert.ToInt32(key.GetValue(ValuePort, 3389));
          UserAuthentication = Convert.ToInt32(key.GetValue(ValueNla, 0));
          SecurityLayer = Convert.ToInt32(key.GetValue(ValueSecurity, 0));
          ShadowOptions = Convert.ToInt32(key.GetValue(ValueShadow, 0));
        }
      }
 
      using (var key = Registry.LocalMachine.OpenSubKey(RegWinLogonKey)) {
        if (key != null)
          DontDisplayLastUser = Convert.ToInt32(key.GetValue(ValueDontDisplayLastUserName, 0)) != 0;
      }
    }

    internal void SaveSettings() {

      using (var key = Registry.LocalMachine.OpenSubKey(RegKey, writable: true)) {
        if (key != null) {
          key.SetValue(ValueSingleSession, SingleSessionPerUser ? 1 : 0, RegistryValueKind.DWord);
          key.SetValue(ValueDenyTsConnections, AllowTsConnections ? 0 : 1, RegistryValueKind.DWord);
          key.SetValue(ValueHonorLegacy, HonorLegacy ? 1 : 0, RegistryValueKind.DWord);
        }
      }
     
      using (var key = Registry.LocalMachine.OpenSubKey(RegRdpKey, true)) {
        if (key != null) {
          key.SetValue(ValuePort, RdpPort, RegistryValueKind.DWord);
          key.SetValue(ValueNla, UserAuthentication, RegistryValueKind.DWord);
          key.SetValue(ValueSecurity, SecurityLayer, RegistryValueKind.DWord);
          key.SetValue(ValueShadow, ShadowOptions, RegistryValueKind.DWord);
        }
      }

      using (var key = Registry.LocalMachine.CreateSubKey(RegTsKey)) {
        key?.SetValue(ValueShadow, ShadowOptions, RegistryValueKind.DWord);
      }

      using (var key = Registry.LocalMachine.CreateSubKey(RegWinLogonKey)) {
        key?.SetValue(ValueDontDisplayLastUserName, DontDisplayLastUser ? 1 : 0, RegistryValueKind.DWord);
      }
    }
 
    internal short CheckWrapperInstalled() {
      WrapperPath = string.Empty;
      try {
        using (var serviceKey = Registry.LocalMachine.OpenSubKey(RegTermServiceKey)) {
          if (serviceKey == null)
            return -1;
          var termServiceHost = serviceKey.GetValue("ImagePath") as string;
          if (string.IsNullOrWhiteSpace(termServiceHost) || !termServiceHost.ToLower().Contains("svchost.exe"))
            return 2;
        }

        string termServicePath;
        using (var paramKey = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters")) {
          if (paramKey == null)
            return -1;

          termServicePath = paramKey.GetValue("ServiceDll") as string;
          if (string.IsNullOrWhiteSpace(termServicePath))
            return -1;
        }

        var lowerPath = termServicePath.ToLower();
        if (!lowerPath.Contains(TermSrvName) && !lowerPath.Contains(RdpWrapDllName))
          return 2;

        if (!lowerPath.Contains(RdpWrapDllName)) return 0;
        WrapperPath = termServicePath;
        return 1;
      }
      catch {
        return -1;
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
    
    internal void GenerateIniFile(string destFilePath, bool executeCleanup = true) {

      string iniFile = null;
      string offsetFinder = null;
      string zydis = null;
      try {
        logger.Log("Generating config...");
        var workingDir = Path.GetTempPath();
        iniFile = ExtractResourceFile(RdpWrapIniName, workingDir, true);
        offsetFinder = ExtractResourceFile("RDPWrapOffsetFinder.exe", workingDir);
        zydis = ExtractResourceFile("Zydis.dll", workingDir);
        var p = StartProcess("cmd", $"/c \"{offsetFinder}\" >> {RdpWrapIniName} & exit", workingDir);
        p.WaitForExit();
        logger.Log(" Done", Logger.StateKind.Info, false);
      }
      catch (Exception ex) {
        var message = "Failed to generate config: " + ex.Message;
        logger.Log(message, Logger.StateKind.Error);
        OnError?.Invoke(message);
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
        OnError?.Invoke(message);
      }
    }

    internal void Install() {
      Directory.CreateDirectory(WrapperFolderPath);
      logger.Log("Folder created: " + WrapperFolderPath);

      AddDirectorySecurity(WrapperFolderPath, [
        "S-1-5-18", // Local System account
        "S-1-5-6", // Service group
        "S-1-5-32-545", // SID for "Users"
      ] , FileSystemRights.FullControl, AccessControlType.Allow);

      var rdpWrap = ExtractResourceFile(RdpWrapDllName, WrapperFolderPath);
      logger.Log("Extracted rdpWrap.dll -> " + rdpWrap);

      logger.Log("Configuring service library...");
      using var reg = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true);
      if (reg == null) {
        logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
        return;
      }
      reg.SetValue("ServiceDll", rdpWrap, RegistryValueKind.ExpandString);
      logger.Log(" Done", Logger.StateKind.Info, false);

      GenerateIniFile(Path.Combine(WrapperFolderPath, RdpWrapIniName));
    }

    internal void Uninstall() {
      logger.Log("Resetting service library...");
      using var reg = Registry.LocalMachine.OpenSubKey(RegTermServiceKey + "\\Parameters", writable: true);
      if (reg == null) {
        logger.Log($"OpenKey error (code {Marshal.GetLastWin32Error()}).", Logger.StateKind.Error);
        return;
      }
      reg.SetValue("ServiceDll", @"%SystemRoot%\System32\termsrv.dll", RegistryValueKind.ExpandString);
      logger.Log(" Done", Logger.StateKind.Info, false);

      var serviceState = serviceHelper.GetServiceState(RdpServiceName); 
      if (serviceState is ServiceControllerStatus.Running)
        serviceHelper.StopService(RdpServiceName, TimeSpan.FromSeconds(10));

      logger.Log("Removed folder: " + WrapperFolderPath);
      Directory.Delete(WrapperFolderPath, true);

      if (serviceState.HasValue)
        serviceHelper.StartService(RdpServiceName, TimeSpan.FromSeconds(10));
    }
    
    #region supplementary methods
    
    private string ExtractResourceFile(string resourceName, string path, bool deleteExisting = false) {
      var filePath = Path.Combine(path, resourceName);
      if (File.Exists(filePath)) {
        if (!deleteExisting) {
          return filePath; //todo: delete
        }
        SafeDeleteFile(filePath);
      }
      try {
        var type = GetType();
        var scriptsPath = $"{type.Namespace}.externals.{resourceName}";
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