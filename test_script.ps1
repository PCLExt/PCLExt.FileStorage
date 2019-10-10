if($isWindows)
{
	dotnet tool install -g coveralls.net
	choco install codecov

	(New-Object System.Net.WebClient).DownloadFile("https://github.com/OpenCover/opencover/releases/download/4.7.922/opencover.4.7.922.zip", "$(Get-Location)/opencover.4.7.922.zip")
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	function Unzip
	{
		param([string]$zipfile, [string]$outpath)

		[System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
	}
	Unzip "$(Get-Location)/opencover.4.7.922.zip" "$(Get-Location)/opencover.4.7.922"
	
	choco install nunit-console-runner
	opencover.4.7.922/OpenCover.Console.exe -filter:"+[PCLExt.*]* -[PCLExt.FileStorage.NetFX.Test*]*" -register:user -target:"nunit3-console.exe" -targetargs:"/domain:single test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll" -output:coverage_netfx.xml
	csmacnz.coveralls --opencover -i coverage_netfx.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f coverage_netfx.xml

	# https://github.com/dotnet/sdk/issues/1514
	#dotnet add test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj package coverlet.msbuild --no-restore
	#&('dotnet') ('test', 'test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=opencover', '/p:Exclude=\"[NUnit*]*,[PCLExt.FileStorage.Core.Test*]*\"', '--logger:"trx;LogFileName=../core-result.trx"')
	#dotnet add test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj package coverlet.collector --no-restore
	dotnet publish test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj -f netcoreapp3.0 --no-restore
	dotnet vstest test/PCLExt.FileStorage.Core.Test/bin/$env:CONFIGURATION/netcoreapp3.0/publish/PCLExt.FileStorage.Core.Test.dll --collect:"XPlat Code Coverage" --logger:"trx;LogFileName=../core-result.trx" --ResultsDirectory:"test/PCLExt.FileStorage.Core.Test/TestResults"
	Get-ChildItem "test/PCLExt.FileStorage.Core.Test/TestResults" -Recurse -File -Filter "coverage.cobertura.xml" | Sort-Object -Property LastWriteTime | foreach { copy $_.FullName "test/PCLExt.FileStorage.Core.Test" }
	csmacnz.coveralls --opencover -i test/PCLExt.FileStorage.Core.Test/coverage.opencover.xml --repoToken $env:COVERALLS_REPO_TOKEN
	codecov -f test/PCLExt.FileStorage.Core.Test/coverage.opencover.xml
	(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", "$(Get-Location )/test/PCLExt.FileStorage.Core.Test/core-result.trx")
}
if($isLinux)
{
	wget "https://github.com/nunit/nunit-console/releases/download/v3.10/NUnit.Console-3.10.0.zip"
	unzip "NUnit.Console-3.10.0.zip" -d "nunit"
	
	mono nunit/bin/net35/nunit3-console.exe ./test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll --result="fx-result.xml"

	dotnet vstest ./test/PCLExt.FileStorage.Core.Test/bin/$env:CONFIGURATION/netcoreapp3.0/PCLExt.FileStorage.Core.Test.dll --logger:"trx;LogFileName=../core-result.trx"
	
	# manually upload test results
	(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'fx-result.xml'))
	(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'core-result.trx'))
}
