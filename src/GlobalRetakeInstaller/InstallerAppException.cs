namespace GRModInstaller;

public enum InstallerErrorKind
{
    InvalidArchivePath,
    GitHubEmptyPayload,
    GitHubMissingAssets
}

public sealed class InstallerAppException : Exception
{
    public InstallerAppException(InstallerErrorKind kind)
        : base(kind.ToString())
    {
        Kind = kind;
    }

    public InstallerErrorKind Kind { get; }
}
