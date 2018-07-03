cd $env:APPVEYOR_BUILD_FOLDER
cd common
nuget pack -Version $env:APPVEYOR_BUILD_VERSION
Push-AppveyorArtifact "PCLExt.FileStorage.*.nupkg"