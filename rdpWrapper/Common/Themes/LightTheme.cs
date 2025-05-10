using System.Drawing;

namespace sergiye.Common {
  internal class LightTheme : Theme {

    public static readonly Color DefaultStatusOkColor = Color.ForestGreen;
    public static readonly Color DefaultStatusInfoColor = Color.Gray;
    public static readonly Color DefaultStatusErrorColor = Color.Red;

    public LightTheme() : base("light", "Light") {
      ForegroundColor = Color.Black;
      BackgroundColor = Color.White;
      HyperlinkColor = Color.OrangeRed;
      SelectedForegroundColor = ForegroundColor;
      SelectedBackgroundColor = Color.CornflowerBlue;
      LineColor = Color.FromArgb(247, 247, 247);
      StrongLineColor = Color.FromArgb(209, 209, 209);
      WindowTitlebarFallbackToImmersiveDarkMode = false;
      StatusOkColor = DefaultStatusOkColor;
      StatusInfoColor = DefaultStatusInfoColor;
      StatusErrorColor = DefaultStatusErrorColor;
    }
  }
}
