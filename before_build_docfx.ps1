if(-Not $env:APPVEYOR_PULL_REQUEST_TITLE) # is not a pull request
{
	git checkout $env:APPVEYOR_REPO_BRANCH -q
	choco install docfx -y
	# choco install nuget.commandline -y
}