if($isWindows)
{
	nunit3-console ./test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll --result="fx-result.xml"
}
if($isLinux)
{
	wget "https://github.com/nunit/nunit-console/releases/download/3.8/NUnit.Console-3.8.0.zip"
	unzip "NUnit.Console-3.8.0.zip" "nunit"
	
	mono nunit/nunit3-console ./test/PCLExt.FileStorage.NetFX.Test/bin/$env:CONFIGURATION/PCLExt.FileStorage.NetFX.Test.dll --result="fx-result.xml"
}

dotnet vstest ./test/PCLExt.FileStorage.Core.Test/bin/$env:CONFIGURATION/netcoreapp2.0/PCLExt.FileStorage.Core.Test.dll --logger:"trx;LogFileName=../core-result.trx"