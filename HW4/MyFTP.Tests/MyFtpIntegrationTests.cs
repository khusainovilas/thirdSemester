// <copyright file="MyFtpIntegrationTests.cs" company="khusainovilas">
// Copyright (c) khusainovilas. All rights reserved.
// </copyright>

namespace MyFTP.Tests;

using System.Diagnostics;
using System.Security.Cryptography;

/// <summary>
/// End-to-end integration tests for the MyFTP.
/// </summary>
public class MyFtpIntegrationTests
{
    private const int Port = 12345;
    private const string ClientExe = @"..\..\..\..\MyFTP.client\bin\Debug\net9.0\MyFTP.client.exe";
    private const string ServerExe = @"..\..\..\..\MyFTP\bin\Debug\net9.0\MyFTP.exe";
    private const string ServerRoot = @"..\..\..\..\MyFTP";

    private Process? server;

    /// <summary>
    /// Starts the server.
    /// </summary>
    [OneTimeSetUp]
    public void StartServer()
    {
        KillAllProcesses();

        var serverPath = Path.GetFullPath(ServerExe);
        if (!File.Exists(serverPath))
        {
            Assert.Inconclusive($"\nServer not found: {serverPath}");
        }

        this.server = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = serverPath,
                Arguments = Port.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetFullPath(ServerRoot),
            },
        };

        this.server.Start();
        Thread.Sleep(3000);
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    [OneTimeTearDown]
    public void StopServer()
    {
        if (this.server is { HasExited: false })
        {
            try
            {
                this.server.Kill();
            }
            catch
            {
                // ignored
            }
        }

        this.server?.Dispose();
        KillAllProcesses();
    }

    /// <summary>
    /// Clears the folder.
    /// </summary>
    [SetUp]
    public void Cleanup()
    {
        var dir = TestContext.CurrentContext.TestDirectory;
        foreach (var file in new[] { "file.txt", "pin.jpg", "123.docx", "111.txt" })
        {
            var path = Path.Combine(dir, file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// Checking the List command for the directory ./Test.
    /// </summary>
    [Test]
    public void IntegrationTest_ReturnsCorrectFileList()
    {
        var output = RunClient("1 ./Test");

        var line = output.Split('\n')
            .Select(l => l.Trim())
            .FirstOrDefault(l => l.Contains("./Test/pin.jpg") && l.Contains("./Test/dir"));

        Assert.That(line, Is.Not.Null, $"List didn't work! Full output:\n{output}");
        StringAssert.Contains("./Test/pin.jpg false", line);
        StringAssert.Contains("./Test/dir true", line);
    }

    /// <summary>
    /// Checking the download of a .txt file.
    /// </summary>
    [Test]
    public void IntegrationTest_ExistingTextFile_DownloadsSuccessfully()
    {
        RunClient("2 ./Test/file.txt");
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "file.txt");
        Assert.That(File.Exists(path), Is.True, "file.txt did not download!");
    }

    /// <summary>
    /// Checking the download of a .jpg file.
    /// </summary>
    [Test]
    public void IntegrationTest_ExistingJpgFile_DownloadsSuccessfully()
    {
        var original = Path.Combine(ServerRoot, "Test", "pin.jpg");
        var originalHash = GetSha256(original);

        RunClient("2 ./Test/pin.jpg");

        var downloaded = Path.Combine(TestContext.CurrentContext.TestDirectory, "pin.jpg");
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(downloaded), Is.True, "pin.jpg did not download!");
            Assert.That(GetSha256(downloaded), Is.EqualTo(originalHash));
        });
    }

    /// <summary>
    /// Checking the download of a .docx file.
    /// </summary>
    [Test]
    public void IntegrationTest_ExistingDocxFile_DownloadsSuccessfully()
    {
        var original = Path.Combine(ServerRoot, "Test", "123.docx");
        var originalHash = GetSha256(original);

        RunClient("2 ./Test/123.docx");

        var downloaded = Path.Combine(TestContext.CurrentContext.TestDirectory, "123.docx");
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(downloaded), Is.True, "123.docx did not download!");
            Assert.That(GetSha256(downloaded), Is.EqualTo(originalHash));
        });
    }

    /// <summary>
    /// Checking the behavior when requesting a non-existent file.
    /// </summary>
    [Test]
    public void IntegrationTest_NonExistentFile_ReturnsMinusOne()
    {
        var output = RunClient("2 ./Test/nonexistent.txt");
        StringAssert.Contains("-1", output);
    }

    private static string RunClient(string command)
    {
        var clientPath = Path.GetFullPath(ClientExe);
        var psi = new ProcessStartInfo
        {
            FileName = clientPath,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = TestContext.CurrentContext.TestDirectory,
        };

        using var client = Process.Start(psi)!;
        client.StandardInput.WriteLine(command);
        client.StandardInput.WriteLine("exit");
        client.StandardInput.Flush();

        var output = client.StandardOutput.ReadToEnd();
        var error = client.StandardError.ReadToEnd();

        client.WaitForExit(20000);
        if (!client.HasExited)
        {
            client.Kill();
        }

        if (client.ExitCode != 0)
        {
            Assert.Fail($"The client has fallen!\nError: {error}\nOutput: {output}");
        }

        return output;
    }

    private static string GetSha256(string path)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(path);
        return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static void KillAllProcesses()
    {
        foreach (var p in Process.GetProcesses())
        {
            if (p.ProcessName.Contains("MyFTP"))
            {
                try
                {
                    p.Kill();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}