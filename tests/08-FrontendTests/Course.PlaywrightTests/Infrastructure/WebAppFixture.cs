using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace Course.PlaywrightTests.Infrastructure;

public sealed class WebAppFixture : IAsyncLifetime
{
    private Process? _apiProcess;
    private Process? _webProcess;
    private ProcessOutput? _apiOutput;
    private ProcessOutput? _webOutput;

    public string ApiBaseUrl { get; private set; } = string.Empty;

    public string WebBaseUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        var root = FindRepositoryRoot();
        var configuration = GetCurrentConfiguration();
        var apiPort = GetFreePort();
        var webPort = GetFreePort();

        ApiBaseUrl = $"http://127.0.0.1:{apiPort}";
        WebBaseUrl = $"http://127.0.0.1:{webPort}";

        _apiOutput = new ProcessOutput("Course.Api");
        _webOutput = new ProcessOutput("Course.Web");

        _apiProcess = StartDotnetApp(
            root,
            "src/Course.Api/Course.Api.csproj",
            configuration,
            apiPort,
            new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Testing",
                ["DOTNET_ENVIRONMENT"] = "Testing"
            },
            _apiOutput);

        await WaitForHealthAsync($"{ApiBaseUrl}/health", _apiProcess, _apiOutput);

        _webProcess = StartDotnetApp(
            root,
            "src/Course.Web/Course.Web.csproj",
            configuration,
            webPort,
            new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Testing",
                ["DOTNET_ENVIRONMENT"] = "Testing",
                ["CourseApi__BaseUrl"] = ApiBaseUrl
            },
            _webOutput);

        await WaitForHealthAsync($"{WebBaseUrl}/health", _webProcess, _webOutput);
    }

    public Task DisposeAsync()
    {
        StopProcess(_webProcess);
        StopProcess(_apiProcess);
        return Task.CompletedTask;
    }

    private static DirectoryInfo FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TestingBackendCourse.sln")))
            {
                return directory;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro TestingBackendCourse.sln.");
    }

    private static string GetCurrentConfiguration()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var releaseSegment = $"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}";

        return baseDirectory.Contains(releaseSegment, StringComparison.OrdinalIgnoreCase)
            ? "Release"
            : "Debug";
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static Process StartDotnetApp(
        DirectoryInfo root,
        string projectPath,
        string configuration,
        int port,
        IReadOnlyDictionary<string, string?> environment,
        ProcessOutput output)
    {
        var fullProjectPath = Path.Combine(root.FullName, projectPath);
        if (!File.Exists(fullProjectPath))
        {
            throw new FileNotFoundException($"No se encontro el proyecto para Playwright: {fullProjectPath}", fullProjectPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root.FullName,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--configuration");
        startInfo.ArgumentList.Add(configuration);
        startInfo.ArgumentList.Add("--no-build");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add($"http://127.0.0.1:{port}");

        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        startInfo.Environment["DOTNET_NOLOGO"] = "1";

        foreach (var item in environment)
        {
            startInfo.Environment[item.Key] = item.Value;
        }

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, args) => output.Add(args.Data);
        process.ErrorDataReceived += (_, args) => output.Add(args.Data);

        if (!process.Start())
        {
            throw new InvalidOperationException($"No se pudo iniciar {projectPath}.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }

    private static async Task WaitForHealthAsync(string healthUrl, Process process, ProcessOutput output)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var deadline = DateTimeOffset.UtcNow.AddSeconds(90);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException($"{healthUrl} termino antes de estar disponible.{Environment.NewLine}{output}");
            }

            try
            {
                using var response = await client.GetAsync(healthUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"{healthUrl} no estuvo disponible a tiempo.{Environment.NewLine}{output}");
    }

    private static void StopProcess(Process? process)
    {
        if (process is null)
        {
            return;
        }

        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(5000);
            }
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            process.Dispose();
        }
    }

    private sealed class ProcessOutput
    {
        private const int Limit = 200;
        private readonly object _gate = new();
        private readonly Queue<string> _lines = new();
        private readonly string _name;

        public ProcessOutput(string name)
        {
            _name = name;
        }

        public void Add(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            lock (_gate)
            {
                _lines.Enqueue(line);
                while (_lines.Count > Limit)
                {
                    _lines.Dequeue();
                }
            }
        }

        public override string ToString()
        {
            lock (_gate)
            {
                return $"{_name} output:{Environment.NewLine}{string.Join(Environment.NewLine, _lines)}";
            }
        }
    }
}
