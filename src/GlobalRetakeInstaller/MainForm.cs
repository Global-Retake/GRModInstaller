using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows.Forms;

namespace GRModInstaller;

public sealed class MainForm : Form
{
    private const string DefaultInstallPath = @"C:\Program Files (x86)\Steam\steamapps\common\csgo legacy";

    private readonly HttpClient _httpClient = CreateHttpClient();
    private readonly ReleaseService _releaseService;

    private readonly Label _titleLabel;
    private readonly Label _introLabel;
    private readonly Label _languageLabel;
    private readonly ComboBox _languageComboBox;
    private readonly GroupBox _releaseGroup;
    private readonly Label _releaseLabel;
    private readonly Label _releaseDetailsLabel;
    private readonly GroupBox _modeGroup;
    private readonly RadioButton _fullRadioButton;
    private readonly RadioButton _patchRadioButton;
    private readonly Label _modeLabel;
    private readonly GroupBox _destinationGroup;
    private readonly Label _destinationLabel;
    private readonly TextBox _pathTextBox;
    private readonly Button _browseButton;
    private readonly Button _refreshButton;
    private readonly Button _installButton;
    private readonly Button _exitButton;
    private readonly ProgressBar _progressBar;
    private readonly Label _statusLabel;

    private ReleaseInfo? _currentRelease;
    private InstallerLanguage _currentLanguage;
    private CultureInfo _currentCulture;
    private bool _isBusy;
    private bool _isApplyingLanguage;
    private ReleaseUiState _releaseUiState = ReleaseUiState.Loading;
    private TextId _statusTextId = TextId.StatusLoading;
    private object[] _statusArgs = [];

    public MainForm()
    {
        _releaseService = new ReleaseService(_httpClient);
        _currentLanguage = LocalizationCatalog.Resolve(CultureInfo.CurrentUICulture);
        _currentCulture = LocalizationCatalog.GetCulture(_currentLanguage);

        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = true;
        ShowInTaskbar = true;
        ClientSize = new Size(680, 468);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        _titleLabel = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(16, 14)
        };

        _languageLabel = new Label
        {
            AutoSize = true,
            Location = new Point(428, 17)
        };

        _languageComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(488, 13),
            Size = new Size(176, 23)
        };
        _languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

        _introLabel = new Label
        {
            AutoSize = false,
            Location = new Point(16, 42),
            Size = new Size(648, 34)
        };

        _releaseGroup = new GroupBox
        {
            Location = new Point(16, 84),
            Size = new Size(648, 84)
        };

        _releaseLabel = new Label
        {
            AutoSize = true,
            Location = new Point(14, 24)
        };

        _releaseDetailsLabel = new Label
        {
            AutoSize = false,
            Location = new Point(14, 44),
            Size = new Size(620, 30)
        };

        _releaseGroup.Controls.Add(_releaseLabel);
        _releaseGroup.Controls.Add(_releaseDetailsLabel);

        _modeGroup = new GroupBox
        {
            Location = new Point(16, 176),
            Size = new Size(648, 94)
        };

        _fullRadioButton = new RadioButton
        {
            AutoSize = true,
            Checked = true,
            Location = new Point(16, 26)
        };
        _fullRadioButton.CheckedChanged += InstallModeChanged;

        _patchRadioButton = new RadioButton
        {
            AutoSize = true,
            Location = new Point(160, 26)
        };
        _patchRadioButton.CheckedChanged += InstallModeChanged;

        _modeLabel = new Label
        {
            AutoSize = false,
            Location = new Point(16, 52),
            Size = new Size(615, 30)
        };

        _modeGroup.Controls.Add(_fullRadioButton);
        _modeGroup.Controls.Add(_patchRadioButton);
        _modeGroup.Controls.Add(_modeLabel);

        _destinationGroup = new GroupBox
        {
            Location = new Point(16, 278),
            Size = new Size(648, 92)
        };

        _destinationLabel = new Label
        {
            AutoSize = true,
            Location = new Point(16, 28)
        };

        _pathTextBox = new TextBox
        {
            Location = new Point(16, 50),
            Size = new Size(526, 23),
            Text = DefaultInstallPath
        };
        _pathTextBox.TextChanged += (_, _) => UpdateControlState();

        _browseButton = new Button
        {
            Location = new Point(552, 48),
            Size = new Size(80, 25)
        };
        _browseButton.Click += BrowseButton_Click;

        _destinationGroup.Controls.Add(_destinationLabel);
        _destinationGroup.Controls.Add(_pathTextBox);
        _destinationGroup.Controls.Add(_browseButton);

        _progressBar = new ProgressBar
        {
            Location = new Point(16, 380),
            Size = new Size(648, 18),
            Style = ProgressBarStyle.Marquee
        };

        _statusLabel = new Label
        {
            AutoSize = false,
            Location = new Point(16, 404),
            Size = new Size(648, 24)
        };

        _refreshButton = new Button
        {
            Location = new Point(344, 434),
            Size = new Size(94, 27)
        };
        _refreshButton.Click += RefreshButton_Click;

        _installButton = new Button
        {
            Location = new Point(454, 434),
            Size = new Size(94, 27),
            Enabled = false
        };
        _installButton.Click += InstallButton_Click;

        _exitButton = new Button
        {
            Location = new Point(570, 434),
            Size = new Size(94, 27)
        };
        _exitButton.Click += (_, _) => Close();

        Controls.Add(_titleLabel);
        Controls.Add(_languageLabel);
        Controls.Add(_languageComboBox);
        Controls.Add(_introLabel);
        Controls.Add(_releaseGroup);
        Controls.Add(_modeGroup);
        Controls.Add(_destinationGroup);
        Controls.Add(_progressBar);
        Controls.Add(_statusLabel);
        Controls.Add(_refreshButton);
        Controls.Add(_installButton);
        Controls.Add(_exitButton);

        PopulateLanguageList();
        ApplyLanguage(_currentLanguage);

        Shown += async (_, _) => await LoadLatestReleaseAsync(showDialogOnError: true);
        FormClosed += (_, _) => _httpClient.Dispose();
    }

    private InstallMode SelectedMode => _patchRadioButton.Checked ? InstallMode.Patch : InstallMode.Full;

    private async void RefreshButton_Click(object? sender, EventArgs e)
    {
        await LoadLatestReleaseAsync(showDialogOnError: true);
    }

    private void BrowseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = GetText(TextId.BrowseDialogDescription),
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true,
            SelectedPath = Directory.Exists(_pathTextBox.Text) ? _pathTextBox.Text : DefaultInstallPath
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _pathTextBox.Text = dialog.SelectedPath;
        }
    }

    private void InstallModeChanged(object? sender, EventArgs e)
    {
        UpdateModeDescription();
    }

    private async void InstallButton_Click(object? sender, EventArgs e)
    {
        await RunInstallAsync();
    }

    private async Task LoadLatestReleaseAsync(bool showDialogOnError)
    {
        try
        {
            _releaseUiState = ReleaseUiState.Loading;
            SetBusyState(true, TextId.StatusCheckingRelease, true, 0);
            UpdateReleaseText();

            _currentRelease = await _releaseService.GetLatestReleaseAsync();
            _releaseUiState = ReleaseUiState.Ready;
            UpdateReleaseText();

            SetBusyState(false, TextId.StatusReady, false, 0);
        }
        catch (Exception ex)
        {
            _currentRelease = null;
            _releaseUiState = ReleaseUiState.Failed;
            UpdateReleaseText();

            var errorMessage = GetErrorMessage(ex);
            SetBusyState(false, TextId.ReleaseLookupFailedPrefix, false, 0, errorMessage);

            if (showDialogOnError)
            {
                MessageBox.Show(
                    this,
                    Format(TextId.ReleaseLookupFailedDialogFormat, errorMessage),
                    GetText(TextId.ReleaseLookupFailedTitle),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    private async Task RunInstallAsync()
    {
        if (string.IsNullOrWhiteSpace(_pathTextBox.Text))
        {
            MessageBox.Show(
                this,
                GetText(TextId.MissingDestinationMessage),
                GetText(TextId.MissingDestinationTitle),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (_currentRelease is null)
        {
            await LoadLatestReleaseAsync(showDialogOnError: true);

            if (_currentRelease is null)
            {
                return;
            }
        }

        var selectedAsset = _currentRelease.GetAsset(SelectedMode);
        var installPath = Path.GetFullPath(_pathTextBox.Text.Trim());
        var tempRoot = Path.Combine(Path.GetTempPath(), "GlobalRetakeInstaller", Guid.NewGuid().ToString("N"));
        var archivePath = Path.Combine(tempRoot, selectedAsset.Name);

        try
        {
            SetBusyState(true, TextId.StatusPreparingInstall, false, 0);

            Directory.CreateDirectory(tempRoot);
            Directory.CreateDirectory(installPath);

            var progress = new Progress<InstallProgress>(UpdateProgress);

            await DownloadAssetAsync(selectedAsset, archivePath, progress);
            await ExtractArchiveAsync(archivePath, installPath, progress);

            SetBusyState(false, TextId.InstallCompleteStatus, false, 100);

            MessageBox.Show(
                this,
                Format(TextId.InstallCompleteDialogFormat, _currentRelease.TagName, installPath),
                GetText(TextId.InstallCompleteTitle),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            var errorMessage = GetErrorMessage(ex);
            SetBusyState(false, TextId.InstallFailedPrefix, false, _progressBar.Value, errorMessage);

            MessageBox.Show(
                this,
                Format(TextId.InstallFailedDialogFormat, errorMessage),
                GetText(TextId.InstallFailedTitle),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            TryDeleteDirectory(tempRoot);
            UpdateControlState();
        }
    }

    private void UpdateProgress(InstallProgress progress)
    {
        SetStatus(progress.StatusTextId, progress.IsIndeterminate, (int)Math.Round(progress.Percent), progress.Arguments);
    }

    private void SetBusyState(bool isBusy, TextId statusTextId, bool indeterminate, int progressPercent, params object[] args)
    {
        _isBusy = isBusy;
        SetStatus(statusTextId, indeterminate, progressPercent, args);
        UpdateControlState();
    }

    private void SetStatus(TextId statusTextId, bool indeterminate, int progressPercent, params object[] args)
    {
        _statusTextId = statusTextId;
        _statusArgs = args;
        _statusLabel.Text = Format(statusTextId, args);

        if (indeterminate)
        {
            _progressBar.Style = ProgressBarStyle.Marquee;
        }
        else
        {
            if (_progressBar.Style != ProgressBarStyle.Continuous)
            {
                _progressBar.Style = ProgressBarStyle.Continuous;
            }

            _progressBar.Value = Math.Clamp(progressPercent, 0, 100);
        }
    }

    private void UpdateControlState()
    {
        var hasPath = !string.IsNullOrWhiteSpace(_pathTextBox.Text);
        var canInstall = !_isBusy && hasPath && _currentRelease is not null;

        _languageComboBox.Enabled = !_isBusy;
        _fullRadioButton.Enabled = !_isBusy;
        _patchRadioButton.Enabled = !_isBusy;
        _pathTextBox.Enabled = !_isBusy;
        _browseButton.Enabled = !_isBusy;
        _refreshButton.Enabled = !_isBusy;
        _installButton.Enabled = canInstall;
        _exitButton.Enabled = !_isBusy;
    }

    private async Task DownloadAssetAsync(ReleaseAsset asset, string destinationPath, IProgress<InstallProgress> progress, CancellationToken cancellationToken = default)
    {
        progress.Report(new InstallProgress(0, true, TextId.StatusDownloadStartingFormat, asset.Name));

        using var response = await _httpClient.GetAsync(asset.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;

        await using var sourceStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

        var buffer = new byte[81920];
        long totalBytesRead = 0;

        while (true)
        {
            var bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken);

            if (bytesRead <= 0)
            {
                break;
            }

            await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalBytesRead += bytesRead;

            if (totalBytes is > 0)
            {
                var ratio = totalBytesRead / (double)totalBytes.Value;
                progress.Report(new InstallProgress(
                    ratio * 80d,
                    false,
                    TextId.StatusDownloadingFormat,
                    asset.Name,
                    ratio));
            }
            else
            {
                progress.Report(new InstallProgress(0, true, TextId.StatusDownloadingUnknownFormat, asset.Name));
            }
        }

        progress.Report(new InstallProgress(80, false, TextId.StatusDownloadComplete));
    }

    private static Task ExtractArchiveAsync(string archivePath, string installPath, IProgress<InstallProgress> progress, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using var archive = ZipFile.OpenRead(archivePath);
            var totalEntries = Math.Max(archive.Entries.Count, 1);
            var installRoot = EnsureTrailingSeparator(Path.GetFullPath(installPath));

            for (var index = 0; index < archive.Entries.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = archive.Entries[index];
                var destinationPath = Path.GetFullPath(Path.Combine(installRoot, entry.FullName));

                if (!destinationPath.StartsWith(installRoot, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InstallerAppException(InstallerErrorKind.InvalidArchivePath);
                }

                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    var destinationDirectory = Path.GetDirectoryName(destinationPath);

                    if (!string.IsNullOrWhiteSpace(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    entry.ExtractToFile(destinationPath, overwrite: true);
                }

                var percent = 80d + ((index + 1) / (double)totalEntries * 20d);
                progress.Report(new InstallProgress(percent, false, TextId.StatusExtractingFormat, index + 1, totalEntries));
            }
        }, cancellationToken);
    }

    private void PopulateLanguageList()
    {
        _isApplyingLanguage = true;
        _languageComboBox.Items.AddRange(LocalizationCatalog.SupportedLanguages.ToArray());
        _languageComboBox.SelectedItem = LocalizationCatalog.SupportedLanguages.First(language => language.Code == _currentLanguage.Code);
        _isApplyingLanguage = false;
    }

    private void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isApplyingLanguage || _languageComboBox.SelectedItem is not InstallerLanguage language)
        {
            return;
        }

        ApplyLanguage(language);
    }

    private void ApplyLanguage(InstallerLanguage language)
    {
        _currentLanguage = language;
        _currentCulture = LocalizationCatalog.GetCulture(language);

        _isApplyingLanguage = true;
        _languageComboBox.SelectedItem = LocalizationCatalog.SupportedLanguages.First(item => item.Code == language.Code);
        _isApplyingLanguage = false;

        Text = GetText(TextId.AppTitle);
        _titleLabel.Text = GetText(TextId.AppTitle);
        _introLabel.Text = GetText(TextId.Intro);
        _languageLabel.Text = GetText(TextId.LanguageLabel);
        _releaseGroup.Text = GetText(TextId.ReleaseGroupTitle);
        _modeGroup.Text = GetText(TextId.InstallTypeGroupTitle);
        _fullRadioButton.Text = GetText(TextId.FullInstall);
        _patchRadioButton.Text = GetText(TextId.PatchUpdate);
        _destinationGroup.Text = GetText(TextId.DestinationGroupTitle);
        _destinationLabel.Text = GetText(TextId.DestinationLabel);
        _browseButton.Text = GetText(TextId.Browse);
        _refreshButton.Text = GetText(TextId.Refresh);
        _installButton.Text = GetText(TextId.Install);
        _exitButton.Text = GetText(TextId.Exit);

        UpdateModeDescription();
        UpdateReleaseText();
        _statusLabel.Text = Format(_statusTextId, _statusArgs);
    }

    private void UpdateModeDescription()
    {
        _modeLabel.Text = SelectedMode == InstallMode.Full
            ? GetText(TextId.FullDescription)
            : GetText(TextId.PatchDescription);
    }

    private void UpdateReleaseText()
    {
        switch (_releaseUiState)
        {
            case ReleaseUiState.Ready when _currentRelease is not null:
                _releaseLabel.Text = Format(TextId.ReleaseLatestFormat, _currentRelease.TagName);
                _releaseDetailsLabel.Text = Format(
                    TextId.ReleaseDetailsFormat,
                    _currentRelease.Title,
                    _currentRelease.PublishedAt.LocalDateTime,
                    FormatSize(_currentRelease.FullAsset.SizeBytes),
                    FormatSize(_currentRelease.PatchAsset.SizeBytes));
                break;

            case ReleaseUiState.Failed:
                _releaseLabel.Text = GetText(TextId.ReleaseLoadFailed);
                _releaseDetailsLabel.Text = GetText(TextId.ReleaseRetry);
                break;

            default:
                _releaseLabel.Text = GetText(TextId.ReleaseChecking);
                _releaseDetailsLabel.Text = GetText(TextId.ReleaseWait);
                break;
        }
    }

    private string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            InstallerAppException { Kind: InstallerErrorKind.InvalidArchivePath } => GetText(TextId.InvalidArchivePath),
            InstallerAppException { Kind: InstallerErrorKind.GitHubEmptyPayload } => GetText(TextId.GitHubEmptyPayload),
            InstallerAppException { Kind: InstallerErrorKind.GitHubMissingAssets } => GetText(TextId.GitHubMissingAssets),
            _ => exception.Message
        };
    }

    private string GetText(TextId id)
    {
        return LocalizationCatalog.Get(_currentLanguage, id);
    }

    private string Format(TextId id, params object[] args)
    {
        return string.Format(_currentCulture, GetText(id), args);
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
        }
    }

    private static string EnsureTrailingSeparator(string path)
    {
        return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    private string FormatSize(long sizeBytes)
    {
        var sizeInMegabytes = sizeBytes / 1024d / 1024d;
        return string.Format(_currentCulture, "{0:0.#} MB", sizeInMegabytes);
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("GlobalRetakeInstaller/1.0");
        return client;
    }

    private enum ReleaseUiState
    {
        Loading,
        Ready,
        Failed
    }

    private readonly record struct InstallProgress(double Percent, bool IsIndeterminate, TextId StatusTextId, params object[] Arguments);
}
