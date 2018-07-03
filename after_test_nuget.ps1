if($env:CONFIGURATION -eq "Release" -and $env:APPVEYOR_REPO_TAG -eq "true")
{
	cd $env:APPVEYOR_BUILD_FOLDER
	cd common
	nuget pack PCLExt.FileStorage.nuspec -Version $env:APPVEYOR_BUILD_VERSION
	$nupkg = (Get-ChildItem PCLExt.FileStorage*.nupkg)[0];
	Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -DeploymentName "PCLExt.FileStorage.nupkg";
}