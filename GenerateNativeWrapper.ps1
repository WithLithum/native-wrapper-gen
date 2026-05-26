param (
    [switch]
    $NoDownload
)

if (!(Test-Path "NWG_WORK" -PathType Container)) {
    $newDir = New-Item -Type Directory -Path 'NWG_WORK'
}
else {
    $newDir = Get-Item -Path 'NWG_WORK'
}

$nativesJsonPath = Join-Path -Path $newDir.FullName -ChildPath 'natives.json'
$wrapperPath = Join-Path -Path $PWD -ChildPath "wrappers\WithLithum.NativeWrapper" -AdditionalChildPath "Natives.{0}.cs"

if (!($NoDownload)) {
    try {
        Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/alloc8or/gta5-nativedb-data/refs/heads/master/natives.json' -OutFile $nativesJsonPath
    }
    catch {
        Write-Error "Unable to download natives.json file." -Category ConnectionError
        exit 2
    }
}

$artefactPath = Join-Path -Path $PWD -ChildPath "NWG_WORK\bin"
$binPath = Join-Path -Path $PWD -ChildPath "bin"

# Ensure bin exists
if (!(Test-Path $binPath -PathType Container)) {
    New-Item -Path $PWD -Name "bin" -ItemType Directory
}

dotnet restore ".\wrappers\WithLithum.NativeWrapper.slnx"
if (!$?) {
    Write-Host -ForegroundColor Red -Object "Restore failed"
    Exit 1
}

function Write-Wrappers {
    param (
        [string]$Generator,
        [string]$Project
    )

    $projectPath = Join-Path -Path $PWD -ChildPath "wrappers\$project"
    $wrapperPath = Join-Path -Path $projectPath -ChildPath "Natives.{0}.cs"

    dotnet run `
        --project "WithLithum.NativeWrapperGen" `
        --configuration Release `
        -v q `
        --no-restore `
        --natives-file "${nativesJsonPath}" `
        --namespace WithLithum.NativeWrapper `
        --class-name Natives `
        --generator "$Generator" `
        --file-name-format "$wrapperPath"

    if (!$?) {
        Write-Host -ForegroundColor Red -Object "Generator for $Project ($Generator) failed"
        Exit 1
    }

    # Execute build
    $csprojPath = Join-Path -Path $projectPath -ChildPath "$($project).csproj"
    $outPath = Join-Path -Path $artefactPath -ChildPath "$generator"

    dotnet build `
        "$csprojPath" `
        --configuration Release `
        --nologo `
        --output $outPath `
        --verbosity minimal

    if (!$?) {
        Write-Host -ForegroundColor Red -Object "Build for $Project ($Generator) failed"
        Exit 1
    }

    # Move nuget packages
    Move-Item -Path "$outPath\*.nupkg" -Destination $binPath -Force

    Write-Output "Created wrappers binaries at $hooksPath"
}

Write-Wrappers -Generator "shvdn" -Project "WithLithum.NativeWrapper"
Write-Wrappers -Generator "rph" -Project "WithLithum.NativeWrapper.RagePluginHook"

# Create final archive
Copy-Item -Path ".\wrappers\WrapperDistReadme.txt" -Destination "$artefactPath\README.txt" -Force
Compress-Archive -Path "$artefactPath\*" -DestinationPath "$binPath\NativeWrapper.zip" -Force

# Offer to open bin path
Write-Output "Success: created outputs at: $binPath"