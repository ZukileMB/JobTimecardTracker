namespace JobTimecardTracker.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        const string apiBaseUrl = "https://localhost:7158";
        Application.Run(new MainForm(apiBaseUrl));
    }
}