namespace MainUI;

internal class Theme
{
    public Color BackgroundColor { get; init; } = SystemColors.Control;
    public Color ForegroundColor { get; init; } = SystemColors.ControlText;
    public Color TextBoxBackgroundColor { get; init; } = SystemColors.ControlLightLight;

    public void ApplyTheme(Control item)
    {
        if (item.InvokeRequired)
        {
            item.Invoke(SetThemeLocal);
        }
        else
        {
            SetThemeLocal();
        }

        return;

        void SetThemeLocal()
        {
            item.BackColor = item is TextBox ? TextBoxBackgroundColor : BackgroundColor;
            item.ForeColor = ForegroundColor;
        }
    }
}