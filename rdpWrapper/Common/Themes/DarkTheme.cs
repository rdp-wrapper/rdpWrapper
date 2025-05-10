using System.Drawing;

namespace sergiye.Common {

  internal class DarkTheme : Theme {

    public static readonly Color DefaultStatusOkColor = Color.LawnGreen;
    public static readonly Color DefaultStatusInfoColor = Color.Gold;
    public static readonly Color DefaultStatusErrorColor = Color.OrangeRed;

    public DarkTheme() : base("dark", "Dark") {
      ForegroundColor = ColorTranslator.FromHtml("#DADADA");
      BackgroundColor = Color.Black;
      HyperlinkColor = Color.OrangeRed;
      SelectedForegroundColor = ColorTranslator.FromHtml("#DADADA");
      SelectedBackgroundColor = ColorTranslator.FromHtml("#2170CF");
      LineColor = Color.FromArgb(38, 38, 38);
      StrongLineColor = Color.FromArgb(53, 53, 53);
      WindowTitlebarFallbackToImmersiveDarkMode = true;
      StatusOkColor = DefaultStatusOkColor;
      StatusInfoColor = DefaultStatusInfoColor;
      StatusErrorColor = DefaultStatusErrorColor;
    }
  }
}
