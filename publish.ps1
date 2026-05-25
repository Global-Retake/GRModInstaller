param(
    [string]$Runtime = "win-x64"
)

$project = Join-Path $PSScriptRoot "src\GlobalRetakeInstaller\GlobalRetakeInstaller.csproj"
$output = Join-Path $PSScriptRoot "artifacts\publish\$Runtime"

dotnet publish $project `
    -c Release `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $output
