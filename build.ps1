if($isWindows)
{
	msbuild PCLExt.FileStorage.sln /verbosity:m
}
if($isLinux)
{
	msbuild src/PCLExt.FileStorage.Abstractions
	msbuild src/PCLExt.FileStorage.Standard
	msbuild src/PCLExt.FileStorage.NetFX
	msbuild build test/PCLExt.FileStorage.Core.Test
	msbuild build test/PCLExt.FileStorage.NetFX.Test
	msbuild test test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj
	msbuild test/PCLExt.FileStorage.NetFX.Test/PCLExt.FileStorage.NetFX.Test.csproj
	nuget install NUnit.ConsoleRunner -Version 3.8.0 -OutputDirectory packages
	mono packages/NUnit.ConsoleRunner.3.8.0/tools/nunit3-console.exe test/PCLExt.FileStorage.NetFX.Test/bin/Debug/PCLExt.FileStorage.NetFX.Test.dll
}
