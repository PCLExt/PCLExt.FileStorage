if($env:CONFIGURATION -eq "Release" -and $env:APPVEYOR_REPO_TAG -eq "true" -and $isWindows)
{
	cd $env:APPVEYOR_BUILD_FOLDER
	cd common
	nuget pack PCLExt.FileStorage.nuspec -Version $env:APPVEYOR_BUILD_VERSION
	
	$nupkg = (Get-ChildItem PCLExt.FileStorage*.nupkg)[0];
	Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -DeploymentName "PCLExt.FileStorage.nupkg";
	
	nuget source Add -Name "PCLExt" -Source "https://nuget.pkg.github.com/PCLExt/index.json" -UserName PCLExt -Password $env:GITHUB_NUGET
	nuget push $nupkg.FullName -Source "PCLExt"
}