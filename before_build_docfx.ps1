if(-Not $env:APPVEYOR_PULL_REQUEST_TITLE -and $env:CONFIGURATION -eq "Release" -and $isWindows) # is not a pull request
{
	git checkout $env:APPVEYOR_REPO_BRANCH -q
	choco install docfx -y
}