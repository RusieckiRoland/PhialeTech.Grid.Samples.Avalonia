using System.Diagnostics;
using NUnit.Framework;
namespace PhialeSamples.NativeChecks;

[NonParallelizable]
public class ScenarioProcessTests
{
    [TestCase("01")]
    [TestCase("02")]
    [TestCase("03")]
    [TestCase("04")]
    [TestCase("05")]
    [TestCase("06")]
    [TestCase("07")]
    [TestCase("08")]
    public async Task IndependentApplication(string example)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "PhialeTech.Grid.Samples.Avalonia.sln")))
            root = root.Parent;
        Assert.That(root, Is.Not.Null);
        var output = Path.Combine(root!.FullName, "artifacts", "verification", "native-isolated", example);
        Directory.CreateDirectory(output);
        var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = root.FullName };
        foreach (var arg in new[]{"test","Tests/Native.Cases/Native.csproj","-c",
#if DEBUG
            "Debug",
#else
            "Release",
#endif
            "--no-build","--no-restore","--filter","FullyQualifiedName~Example"+example,"--logger","trx;LogFileName=case.trx","--results-directory",output})
            start.ArgumentList.Add(arg);
        var screenshots = Path.Combine(root.FullName, "artifacts", "screenshots");
        Directory.CreateDirectory(screenshots);
        start.Environment["PHIALE_DEMO_SCREENSHOTS"] = screenshots;
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException) { process.Kill(true); throw new TimeoutException("Native scenario " + example + " exceeded three minutes."); }
        var log = await stdout + await stderr;
        await File.WriteAllTextAsync(Path.Combine(output, "run.log"), log);
        Assert.That(process.ExitCode, Is.EqualTo(0), log);
    }
}
