nunit3-console ./test/PCLExt.FileStorage.NetFX.Test/bin/Release/PCLExt.FileStorage.NetFX.Test.dll --result="fx-result.xml"

dotnet vstest ./test/PCLExt.FileStorage.Core.Test/bin/Release/netcoreapp2.0/PCLExt.FileStorage.Core.Test.dll --logger:"trx;LogFileName=../core-result.trx"