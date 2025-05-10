using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace sergiye.Common {

  public class PersistentSettings : ISettings {
    private readonly string configFilePath;
    private IDictionary<string, string> settings = new Dictionary<string, string>();

    public PersistentSettings() {
      configFilePath = Path.ChangeExtension(Application.ExecutablePath, ".config");
    }

    public void Load() {
      //old versions configs compatibility
      if (File.Exists(configFilePath)) {
        try {
          var json = File.ReadAllText(configFilePath);
          var data = json.FromJson<IDictionary<string, string>>();
          if (data != null) {
            settings = data;
            isPortable = true;
            return;
          }
        }
        catch (Exception) {
        }
      }

      //read from registry
      using (var reg = Registry.CurrentUser.OpenSubKey(GetAppRegistryKey())) {
        if (reg == null) return;
        foreach (var key in reg.GetValueNames()) {
          var value = reg.GetValue(key, null) as string;
          settings.Add(key, value);
        }
      }
    }

    public void Save() {
      //remove prev registry settings
      Registry.CurrentUser.DeleteSubKeyTree(GetAppRegistryKey(), false);

      if (IsPortable) {
        SaveToFile(configFilePath);
        return;
      }

      try {
        //save to registry
        using (var reg = Registry.CurrentUser.CreateSubKey(GetAppRegistryKey())) {
          if (reg == null) return;
          foreach (var p in settings) {
            reg.SetValue(p.Key, p.Value);
          }
        }

        if (File.Exists(configFilePath)) {
          try {
            File.Delete(configFilePath);
          }
          catch (Exception) {
            //ignore
          }
        }
      }
      catch (Exception) {
        SaveToFile(configFilePath);
      }
    }

    public void SaveToFile(string configFilePath) {
      try {
        settings.ToJsonFile(configFilePath);
      }
      catch (UnauthorizedAccessException) {
        MessageBox.Show("Access to the path '" + configFilePath + "' is denied. " +
          "The current settings could not be saved.",
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch (IOException) {
        MessageBox.Show("The path '" + configFilePath + "' is not writeable. " +
          "The current settings could not be saved.",
          "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private bool isPortable;
    public bool IsPortable {
      get => isPortable;
      set {
        if (isPortable == value) return;
        isPortable = value;
        Save();
      }
    }

    public bool Contains(string name) {
      return settings.ContainsKey(name);
    }

    public void SetValue(string name, string value) {
      if (settings.TryGetValue(name, out var prevValue) && prevValue == value) return;
      settings[name] = value;
      Save();
    }

    public string GetValue(string name, string value) {
      return settings.TryGetValue(name, out var result) ? result : value;
    }

    public void Remove(string name) {
      settings.Remove(name);
      Save();
    }

    public void SetValue(string name, int value) {
      if (settings.TryGetValue(name, out var prevValue) && int.TryParse(prevValue, out var oldValue) && oldValue == value) return;
      settings[name] = value.ToString();
      Save();
    }

    public int GetValue(string name, int value) {
      return settings.TryGetValue(name, out var str) && int.TryParse(str, out var parsedValue) 
        ? parsedValue 
        : value;
    }

    public void SetValue(string name, float value) {
      if (settings.TryGetValue(name, out var prevValue) && float.TryParse(prevValue, out var oldValue) && oldValue == value) return;
      settings[name] = value.ToString(CultureInfo.InvariantCulture);
      Save();
    }

    public float GetValue(string name, float value) {
      return settings.TryGetValue(name, out var str) &&
             float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue)
        ? parsedValue
        : value;
    }

    public double GetValue(string name, double value) {
      return settings.TryGetValue(name, out var str) &&
             double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue)
        ? parsedValue
        : value;
    }

    public void SetValue(string name, bool value) {
      if (settings.TryGetValue(name, out var prevValue) && bool.TryParse(prevValue, out var oldValue) && oldValue == value) return;
      settings[name] = value ? "true" : "false";
      Save();
    }

    public bool GetValue(string name, bool value) {
      return settings.TryGetValue(name, out var str) ? str == "true" : value;
    }

    public void SetValue(string name, Color color) {
      if (settings.TryGetValue(name, out var prevValue) && int.TryParse(prevValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedValue) && Color.FromArgb(parsedValue) == color) return;
      settings[name] = color.ToArgb().ToString("X8");
      Save();
    }

    public Color GetValue(string name, Color value) {
      return settings.TryGetValue(name, out var str) && int.TryParse(str, NumberStyles.HexNumber,
        CultureInfo.InvariantCulture, out var parsedValue)
        ? Color.FromArgb(parsedValue)
        : value;
    }

    private string GetAppRegistryKey() {
      return $"Software\\{Updater.ApplicationCompany}\\{Updater.ApplicationName}";
    }
  }
}
