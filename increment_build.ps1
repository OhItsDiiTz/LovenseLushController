$path = "$PSScriptRoot\BuildNumber.txt"

if (!(Test-Path $path)) {
    Set-Content $path 0
}

$number = Get-Content $path
$number = [int]$number + 1
Set-Content $path $number
Write-Host "New build number: $number"

$path = "$PSScriptRoot\BuildNumber.txt"
$number = [int](Get-Content $path)
$buildInfoFile = "$PSScriptRoot\BuildInfo.cs"
@"
public static class BuildInfo
{
    public const int BuildNumber = $number;
}
"@ | Set-Content $buildInfoFile