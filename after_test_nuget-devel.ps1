if($env:CONFIGURATION -eq "Release") # is not a pull request
{
	cd $env:APPVEYOR_BUILD_FOLDER
	cd common
	nuget pack PCLExt.FileStorage-devel.nuspec -Version $env:APPVEYOR_BUILD_VERSION
	$nupkg = (Get-ChildItem PCLExt.FileStorage-devel*.nupkg)[0];
	Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -DeploymentName "PCLExt.FileStorage-devel.nupkg";
}