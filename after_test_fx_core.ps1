(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'fx-result.xml'))
(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'core-result.trx'))

if($isWindows)
{
	# PCLExt.FileStorage.Core.Test
	choco install codecov
	dotnet tool install -g coveralls.net
	dotnet add test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj package coverlet.msbuild
	dotnet test test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Exclude=[NUnit*]*
	csmacnz.coveralls --opencover -i coverage.opencover.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f coverage.opencover.xml

	# PCLExt.FileStorage.NetFX.Test
	choco install opencover.portable
	choco install nunit-console-runner
	OpenCover.Console -filter:"+[PCLExt.*]* -[PCLExt.FileStorage.NetFX.Test]*" -register:user -target:"nunit3-console.exe" -targetargs:"/domain:single test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll" -output:coverage_netfx.xml
	csmacnz.coveralls --opencover -i coverage_netfx.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f coverage_netfx.xml
}