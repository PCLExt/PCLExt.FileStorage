if($isWindows)
{
	#dotnet tool install -g coveralls.net
	#choco install codecov

	(New-Object System.Net.WebClient).DownloadFile("https://github.com/OpenCover/opencover/releases/download/4.7.922/opencover.4.7.922.zip", "$(Get-Location)/opencover.zip")
	Expand-Archive .\opencover.zip
	(New-Object System.Net.WebClient).DownloadFile("https://github.com/codecov/codecov-exe/releases/download/1.7.2/codecov-win7-x64.zip", "$(Get-Location)/Codecov.zip")
	Expand-Archive .\Codecov.zip
	
	choco install nunit-console-runner
	opencover/OpenCover.Console.exe -filter:"+[PCLExt.*]* -[PCLExt.FileStorage.NetFX.Test*]*" -register:user -target:"nunit3-console.exe" -targetargs:"/domain:single test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll" -output:coverage_netfx.xml
	#csmacnz.coveralls --opencover -i coverage_netfx.xml --repoToken $env:COVERALLS_REPO_TOKEN
	Codecov/codecov.exe -f coverage_netfx.xml

	# https://github.com/dotnet/sdk/issues/1514
	#dotnet add test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj package coverlet.msbuild --no-restore
	#&('dotnet') ('test', 'test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=opencover', '/p:Exclude=\"[NUnit*]*,[PCLExt.FileStorage.Core.Test*]*\"', '--logger:"trx;LogFileName=../core-result.trx"')
	#dotnet add test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj package coverlet.collector --no-restore
	dotnet publish test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj -f netcoreapp2.0 --no-restore
	dotnet vstest test/PCLExt.FileStorage.Core.Test/bin/$env:CONFIGURATION/netcoreapp2.0/publish/PCLExt.FileStorage.Core.Test.dll --collect:"XPlat Code Coverage" --logger:"trx;LogFileName=../core-result.trx" --ResultsDirectory:"test/PCLExt.FileStorage.Core.Test/TestResults"
	Get-ChildItem "test/PCLExt.FileStorage.Core.Test/TestResults" -Recurse -File -Filter "coverage.cobertura.xml" | Sort-Object -Property LastWriteTime | foreach { copy $_.FullName "test/PCLExt.FileStorage.Core.Test" }
	#csmacnz.coveralls --opencover -i test/PCLExt.FileStorage.Core.Test/coverage.cobertura.xml --repoToken $env:COVERALLS_REPO_TOKEN
	Codecov\codecov.exe -f test/PCLExt.FileStorage.Core.Test/coverage.cobertura.xml
	(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", "$(Get-Location )/test/PCLExt.FileStorage.Core.Test/core-result.trx")
}
if($isLinux)
{
	(New-Object System.Net.WebClient).DownloadFile("https://github.com/OpenCover/opencover/releases/download/4.7.922/opencover.4.7.922.zip", "$(Get-Location)/opencover.zip")
	Expand-Archive .\opencover.zip
	(New-Object System.Net.WebClient).DownloadFile("https://github.com/codecov/codecov-exe/releases/download/1.7.2/codecov-linux-x64.zip", "$(Get-Location)/Codecov.zip")
	Expand-Archive .\Codecov.zip

	(New-Object System.Net.WebClient).DownloadFile("https://github.com/nunit/nunit-console/releases/download/v3.10/NUnit.Console-3.10.0.zip", "$(Get-Location)/NUnit.Console.zip")
	Expand-Archive .\NUnit.Console.zip
	
	mono opencover/OpenCover.Console.exe -filter:"+[PCLExt.*]* -[PCLExt.FileStorage.NetFX.Test*]*" -register:user -target:"NUnit.Console/bin/net35/nunit3-console.exe" -targetargs:"/domain:single test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll" -output:coverage_netfx.xml
	dotnet run Codecov/codecov -f coverage_netfx.xml

	dotnet publish test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj -f netcoreapp2.0 --no-restore
	dotnet vstest ./test/PCLExt.FileStorage.Core.Test/bin/$env:CONFIGURATION/netcoreapp2.0/PCLExt.FileStorage.Core.Test.dll --logger:"trx;LogFileName=../core-result.trx"
	Get-ChildItem "test/PCLExt.FileStorage.Core.Test/TestResults" -Recurse -File -Filter "coverage.cobertura.xml" | Sort-Object -Property LastWriteTime | foreach { copy $_.FullName "test/PCLExt.FileStorage.Core.Test" }
	dotnet run Codecov/codecov -f test/PCLExt.FileStorage.Core.Test/coverage.cobertura.xml
	
	# manually upload test results
	#(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'fx-result.xml'))
	#(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($env:APPVEYOR_JOB_ID)", (Resolve-Path 'core-result.trx'))
}
