You have a `Logs` folder and need to create a log file with the current datetime as it's name, e.g. `2017-06-23.13.45.56.log`
```cs
public class LogsFolder : BaseFolder
{
  public LogsFolder() : base(new ApplicationFolder().CreateFolder("Logs", CreationCollisionOption.OpenIfExists)) { }
}

public class LogFile : BaseFile
{
  public LogFile() : base(new LogsFolder().CreateFile($"{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.log", CreationCollisionOption.OpenIfExists)) { }
}

...

using(var fs = new LogFile().Open(FileAccess.ReadAndWrite))
{
  // -- Do something with the log file
}
```
