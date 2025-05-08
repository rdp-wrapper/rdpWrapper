using System.Drawing;

namespace sergiye.Common {

  internal class LightTheme : Theme {
    public LightTheme() : base("light", "Light") {
      ForegroundColor = Color.Black;
      BackgroundColor = Color.White;
      HyperlinkColor = Color.OrangeRed;
      SelectedForegroundColor = ForegroundColor;
      SelectedBackgroundColor = Color.CornflowerBlue;
      LineColor = Color.Orange;
      StrongLineColor = Color.DarkOrange;
      WindowTitlebarFallbackToImmersiveDarkMode = false;
      StatusOkColor = Color.ForestGreen;
      StatusInfoColor = Color.Gray;
      StatusErrorColor = Color.Red;
    }
  }

  internal class DarkTheme : Theme {
    public DarkTheme() : base("dark", "Dark") {
      ForegroundColor = ColorTranslator.FromHtml("#DADADA");
      BackgroundColor = Color.Black;
      HyperlinkColor = ColorTranslator.FromHtml("#90E6E8");
      SelectedForegroundColor = ColorTranslator.FromHtml("#DADADA");
      SelectedBackgroundColor = ColorTranslator.FromHtml("#2170CF");
      LineColor = ColorTranslator.FromHtml("#070A12");
      StrongLineColor = ColorTranslator.FromHtml("#091217");
      WindowTitlebarFallbackToImmersiveDarkMode = true;
      StatusOkColor = Color.LawnGreen;
      StatusInfoColor = Color.Gold;
      StatusErrorColor = Color.OrangeRed;
    }
  }
}
