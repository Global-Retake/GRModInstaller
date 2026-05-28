using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GRModInstaller;

internal sealed record InstallState(
    string? ReleaseTag,
    DateTimeOffset InstalledAtUtc,
    IReadOnlyList<string> InstalledFiles,
    IReadOnlyList<string> InstalledDirectories,
    IReadOnlyList<ManagedExclusion> Exclusions,
    IReadOnlyList<BackedUpFile>? BackedUpFiles = null);

internal sealed record BackedUpFile(string OriginalRelativePath, string BackupRelativePath);

internal static class InstallStateStore
{
    private const string StateFileName = ".grmod-installer-state.json";
    private const string StateRootFolderName = "GlobalRetakeInstaller";
    private const string StateFolderName = "InstallState";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public static bool Exists(string installPath)
    {
        return File.Exists(GetStateFilePath(installPath)) ||
               File.Exists(GetLocalAppDataStateFilePath(installPath)) ||
               File.Exists(GetLegacyStateFilePath(installPath));
    }

    public static async Task<InstallState?> TryLoadAsync(string installPath, CancellationToken cancellationToken = default)
    {
        foreach (var stateFilePath in new[]
        {
            GetStateFilePath(installPath),
            GetLocalAppDataStateFilePath(installPath),
            GetLegacyStateFilePath(installPath)
        })
        {
            if (!File.Exists(stateFilePath))
            {
                continue;
            }

            await using var stream = new FileStream(stateFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            var state = await JsonSerializer.DeserializeAsync<InstallState>(stream, SerializerOptions, cancellationToken);

            return state ?? throw new InstallerAppException(InstallerErrorKind.InvalidInstallState);
        }

        return null;
    }

    public static async Task SaveAsync(string installPath, InstallState state, CancellationToken cancellationToken = default)
    {
        var stateFilePath = GetStateFilePath(installPath);
        Directory.CreateDirectory(Path.GetDirectoryName(stateFilePath)!);

        await using (var stream = new FileStream(stateFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await JsonSerializer.SerializeAsync(stream, state, SerializerOptions, cancellationToken);
        }

        TryHideStateFile(stateFilePath);
    }

    public static void Delete(string installPath)
    {
        foreach (var stateFilePath in new[]
        {
            GetStateFilePath(installPath),
            GetLocalAppDataStateFilePath(installPath),
            GetLegacyStateFilePath(installPath)
        })
        {
            if (File.Exists(stateFilePath))
            {
                File.Delete(stateFilePath);
            }
        }
    }

    public static string GetStateFilePath(string installPath)
    {
        return Path.Combine(GetStateDirectory(), $"{GetInstallPathHash(installPath)}.json");
    }

    private static string GetLocalAppDataStateFilePath(string installPath)
    {
        return Path.Combine(GetLocalAppDataStateDirectory(), $"{GetInstallPathHash(installPath)}.json");
    }

    private static string GetLegacyStateFilePath(string installPath)
    {
        return Path.Combine(Path.GetFullPath(installPath), StateFileName);
    }

    private static string GetStateDirectory()
    {
        var commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        if (string.IsNullOrWhiteSpace(commonApplicationDataPath))
        {
            commonApplicationDataPath = Path.GetTempPath();
        }

        return Path.Combine(commonApplicationDataPath, StateRootFolderName, StateFolderName);
    }

    private static string GetLocalAppDataStateDirectory()
    {
        var localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(localApplicationDataPath))
        {
            localApplicationDataPath = Path.GetTempPath();
        }

        return Path.Combine(localApplicationDataPath, StateRootFolderName, StateFolderName);
    }

    private static string GetInstallPathHash(string installPath)
    {
        var normalizedInstallPath = Path.GetFullPath(installPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .ToUpperInvariant();

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedInstallPath));
        return Convert.ToHexString(hashBytes);
    }

    private static void TryHideStateFile(string stateFilePath)
    {
        try
        {
            File.SetAttributes(stateFilePath, FileAttributes.Hidden | FileAttributes.NotContentIndexed);
        }
        catch
        {
        }
    }
}
