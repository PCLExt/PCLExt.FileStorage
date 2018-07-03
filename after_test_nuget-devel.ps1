if($env:CONFIGURATION -eq "Release") # is not a pull request
{
	cd $env:APPVEYOR_BUILD_FOLDER
	cd common
	nuget pack PCLExt.FileStorage.-develnuspec -Version $env:APPVEYOR_BUILD_VERSION
	Push-AppveyorArtifact "PCLExt.FileStorage.*.nupkg" -FileName nuget
}