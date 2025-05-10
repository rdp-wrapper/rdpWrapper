using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace sergiye.Common {

  internal class CustomTheme : Theme {
    private class ThemeDto {
      public string DisplayName { get; set; }
      public string ForegroundColor { get; set; }
      public string BackgroundColor { get; set; }
      public string HyperlinkColor { get; set; }
      public string SelectedForegroundColor { get; set; }
      public string SelectedBackgroundColor { get; set; }
      public string LineColor { get; set; }
      public string StrongLineColor { get; set; }
      public bool DarkMode { get; set; }
      public string StatusOkColor { get; set; }
      public string StatusInfoColor { get; set; }
      public string StatusErrorColor { get; set; }
    }

    private CustomTheme(string id, ThemeDto theme) : base(id, theme.DisplayName) {
      ForegroundColor = ColorTranslator.FromHtml(theme.ForegroundColor);
      BackgroundColor = ColorTranslator.FromHtml(theme.BackgroundColor);
      HyperlinkColor = ColorTranslator.FromHtml(theme.HyperlinkColor);
      SelectedForegroundColor = ColorTranslator.FromHtml(theme.SelectedForegroundColor);
      SelectedBackgroundColor = ColorTranslator.FromHtml(theme.SelectedBackgroundColor);
      LineColor = ColorTranslator.FromHtml(theme.LineColor);
      StrongLineColor = ColorTranslator.FromHtml(theme.StrongLineColor);
      WindowTitlebarFallbackToImmersiveDarkMode = theme.DarkMode;
      StatusOkColor = theme.StatusOkColor != null
        ? ColorTranslator.FromHtml(theme.StatusOkColor)
        : theme.DarkMode ? DarkTheme.DefaultStatusOkColor : LightTheme.DefaultStatusOkColor;
      StatusInfoColor = theme.StatusInfoColor != null
        ? ColorTranslator.FromHtml(theme.StatusInfoColor)
        : theme.DarkMode ? DarkTheme.DefaultStatusInfoColor : LightTheme.DefaultStatusInfoColor;
      StatusErrorColor = theme.StatusErrorColor != null
        ? ColorTranslator.FromHtml(theme.StatusErrorColor)
        : theme.DarkMode ? DarkTheme.DefaultStatusErrorColor : LightTheme.DefaultStatusErrorColor;
  }

  public static IEnumerable<Theme> GetAllThemes() {
      var assembly = typeof(Theme).Assembly;
      foreach (var type in assembly.GetTypes()) {
        if (type == typeof(Theme) || !typeof(Theme).IsAssignableFrom(type))
          continue;
        var theme = (Theme)type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
        if (theme != null)
          yield return theme;
      }

      var appPath = Path.GetDirectoryName(Updater.CurrentFileLocation);
      var themesPath = Path.Combine(appPath, "Themes");
      var di = new DirectoryInfo(themesPath);
      if (!di.Exists) {
        yield break;
      }
      foreach (var fi in di.GetFiles("*.json", SearchOption.TopDirectoryOnly)) {
        ThemeDto dto = null;
        try {
          var json = File.ReadAllText(fi.FullName);
          dto = json.FromJson<ThemeDto>();
          if (string.IsNullOrEmpty(dto.DisplayName))
            dto.DisplayName = Path.GetFileNameWithoutExtension(fi.Name);
        }
        catch (Exception) {
          //ignore
        }

        if (dto != null)
          yield return new CustomTheme(fi.Name, dto);
      }
    }
  }
}
