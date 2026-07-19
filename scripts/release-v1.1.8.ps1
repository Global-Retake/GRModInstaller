param(
    [string]$Version = "v1.1.8",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "src\GlobalRetakeInstaller\GlobalRetakeInstaller.csproj"
$publishDir = Join-Path $repoRoot "artifacts\publish\$Version\$Runtime"
$notesPath = Join-Path $repoRoot "RELEASE_NOTES_v1.1.8.md"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet was not found in PATH. Install .NET 8 SDK first."
}

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "git was not found in PATH. Install Git first."
}

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw "gh was not found in PATH. Install GitHub CLI first."
}

Set-Location $repoRoot

Write-Host "Publishing installer for $Runtime..."

dotnet publish $projectPath `
    -c Release `
    -r $Runtime `
    --self-contained true `
    /p:PublishSingleFile=true `
    /p:EnableCompressionInSingleFile=true `
    -o $publishDir

$exePath = Join-Path $publishDir "GlobalRetakeInstaller.exe"
if (-not (Test-Path $exePath)) {
    throw "Publish completed but GlobalRetakeInstaller.exe was not found at $exePath"
}

$zipName = "GlobalRetakeInstaller-$Version-$Runtime.zip"
$zipPath = Join-Path $publishDir $zipName

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Write-Host "Creating zip artifact..."
Compress-Archive -Path $exePath -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "Creating git tag $Version..."
git tag $Version

git push origin $Version

Write-Host "Creating GitHub release $Version..."
gh release create $Version $zipPath `
    --title $Version `
    --notes-file $notesPath

Write-Host "Release completed: $Version"
Write-Host "Artifact: $zipPath"
