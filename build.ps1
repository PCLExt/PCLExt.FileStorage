if($isWindows)
{
	msbuild PCLExt.FileStorage.sln /verbosity:m
}
if($isLinux)
{
	#dotnet build src/PCLExt.FileStorage.Abstractions
	#dotnet build src/PCLExt.FileStorage.Standard
	#FrameworkPathOverride=$MONO_BASE_PATH/4.5-api/ dotnet build src/PCLExt.FileStorage.NetFX
	#dotnet build test/PCLExt.FileStorage.Core.Test
	#FrameworkPathOverride=$MONO_BASE_PATH/4.5-api/ dotnet build test/PCLExt.FileStorage.NetFX.Test
	#dotnet test test/PCLExt.FileStorage.Core.Test/PCLExt.FileStorage.Core.Test.csproj
	$env:FrameworkPathOverride="$env:MONO_BASE_PATH/4.5-api/"
	msbuild test/PCLExt.FileStorage.NetFX.Test/PCLExt.FileStorage.NetFX.Test.csproj
	nuget install NUnit.ConsoleRunner -Version 3.8.0 -OutputDirectory packages
	mono packages/NUnit.ConsoleRunner.3.8.0/tools/nunit3-console.exe test/PCLExt.FileStorage.NetFX.Test/bin/Debug/PCLExt.FileStorage.NetFX.Test.dll
}
