param (
    [switch]
    $NoDownload
)

if (!(Test-Path "NWG_WORK" -PathType Container)) {
    $newDir = New-Item -Type Directory -Path 'NWG_WORK'
} else {
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

function Write-Wrappers {
    param (
        [string]$Generator,
        [string]$Project
    )

    $wrapperPath = Join-Path -Path $PWD -ChildPath "wrappers\$project" -AdditionalChildPath "Natives.{0}.cs"

    dotnet run --project "WithLithum.NativeWrapperGen" --configuration Release -v q -- --natives-file "${nativesJsonPath}" --namespace WithLithum.NativeWrapper --class-name Natives --generator "$Generator" --file-name-format "$wrapperPath" --count-time
}

Write-Wrappers -Generator "shvdn" -Project "WithLithum.NativeWrapper"
Write-Wrappers -Generator "rph" -Project "WithLithum.NativeWrapper.RagePluginHook"
