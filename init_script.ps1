if($isWindows)
{
	$env:SOLUTION_NAME="PCLExt.FileStorage"
	
	if ($env:APPVEYOR_REPO_TAG -eq "true")
	{
		Update-AppveyorBuild -Version "$($env:APPVEYOR_REPO_TAG_NAME)"
	}
}
if($isLinux)
{
	$env:SOLUTION_NAME="PCLExt.FileStorage.Linux"
}
