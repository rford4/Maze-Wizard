using System.Diagnostics;

namespace MazeWizard.Presentation.Tests.Fixtures;

[Trait("Category", "Integration")]
public class IntegrationTest
{
    protected static Process StartApplication(string? args)
    {
        var appFilePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
             @"..\..\..\..",
            "MazeWizard",
            "bin",
            "Debug",
            "net10.0-windows",
            "mazewizard.presentation.exe"));

        ProcessStartInfo processStartInfo = new()
        {
            FileName = appFilePath,
            Arguments = args,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        return Process.Start(processStartInfo)
            ?? throw new Exception("Failed to start application.");
    }
}