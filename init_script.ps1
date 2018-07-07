if($isWindows)
{
	$env:APPVEYOR_PROJECT_NAME="PCLExt.FileStorage"
	
	if ($env:APPVEYOR_REPO_TAG -eq "true")
	{
		Update-AppveyorBuild -Version "$($env:APPVEYOR_REPO_TAG_NAME)"
	}
}
if($isLinux)
{
	$env:APPVEYOR_PROJECT_NAME="PCLExt.FileStorage.Linux"
}
