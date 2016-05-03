 $rootDir = $env:APPVEYOR_BUILD_FOLDER
 $buildNumber = $env:APPVEYOR_BUILD_NUMBER
 $solutionFile = "$rootDir\TITcs.SharePoint.Net.sln"
 $srcDir = "$rootDir\NuGet\TITcs.SharePoint"
 $slns = ls "$rootDir\*.sln"
 $packagesDir = "$rootDir\packages"
 $nuspecPath = "$rootDir\NuGet\TITcs.SharePoint\TITcs.SharePoint.nuspec"
 $nugetExe = "$packagesDir\NuGet.CommandLine.2.8.5\tools\NuGet.exe"
 $nupkgPath = "$rootDir\NuGet\TITcs.SharePoint\TITcs.SharePoint.{0}.nupkg"

foreach($sln in $slns) {
   nuget restore $sln
}

[xml]$xml = cat $nuspecPath
$xml.package.metadata.version+=".$buildNumber"
$xml.Save($nuspecPath)

[xml]$xml = cat $nuspecPath
$nupkgPath = $nupkgPath -f $xml.package.metadata.version

nuget pack $nuspecPath -properties "Configuration=$env:configuration;Platform=AnyCPU;Version=$($env:appveyor_build_version)" -OutputDirectory $srcDir 
appveyor PushArtifact $nupkgPath
