using System.Diagnostics;
using Xunit;

public class PerformanceTest
{
    [Fact]
    public void RunK6PerformanceTests()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c .\\RunK6PerformanceTests.bat", // Path to your batch file
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(processStartInfo);
        process.WaitForExit();

        // Optionally, check process exit code to validate if tests passed
        Assert.Equal(0, process.ExitCode);
    }
}