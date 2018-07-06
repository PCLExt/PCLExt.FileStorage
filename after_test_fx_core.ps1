if($isWindows) #no need to do the coverage on Ubuntu
{
	# PCLExt.FileStorage.Core.Test
	choco install codecov
	dotnet tool install -g coveralls.net
	CD $env:APPVEYOR_BUILD_FOLDER
	CD test\PCLExt.FileStorage.Core.Test
	dotnet add package coverlet.msbuild
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Exclude=[NUnit*]*
	csmacnz.coveralls --opencover -i coverage.opencover.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f coverage.opencover.xml

	# PCLExt.FileStorage.NetFX.Test
	CD $env:APPVEYOR_BUILD_FOLDER
	nuget install OpenCover -Version 4.6.519 -OutputDirectory tools
	nuget install NUnit.ConsoleRunner -Version 3.8.0 -OutputDirectory tools
	.\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe -filter:"+[PCLExt.*]* -[PCLExt.FileStorage.NetFX.Test]*" -register:user -target:".\tools\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe" -targetargs:"/domain:single test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll" -output:coverage_netfx.xml
	csmacnz.coveralls --opencover -i coverage_netfx.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f coverage_netfx.xml
}