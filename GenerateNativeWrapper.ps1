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
$wrapperPath = Join-Path -Path $PWD -ChildPath "WithLithum.NativeWrapper" -AdditionalChildPath "Natives.{0}.cs"

if (!($NoDownload)) {
    try {
        Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/alloc8or/gta5-nativedb-data/refs/heads/master/natives.json' -OutFile $nativesJsonPath   
    }
    catch {
        Write-Error "Unable to download natives.json file." -Category ConnectionError
        exit 2
    }
}

dotnet run --project "WithLithum.NativeWrapperGen" --configuration Release -verbosity quiet -- --natives-file "${nativesJsonPath}" --namespace WithLithum.NativeWrapper --class-name Natives --file-name-format "$wrapperPath" --count-time