# JSON File
Class that serializes/deserializes JSON file.

```cs
public abstract class JsonFile : BaseFile
{
    /// <summary>
    /// Ignores readonly properties from BaseFile in the Json file.
    /// </summary>
    protected class BaseFileResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> 
            CreateProperties(Type type, MemberSerialization memberSerialization) =>
            base.CreateProperties(type, memberSerialization)
                .Where(p => p.PropertyName != nameof(Name) &&
                            p.PropertyName != nameof(Path) &&
                            p.PropertyName != nameof(Exists) &&
                            p.PropertyName != nameof(Size)&&
                            p.PropertyName != nameof(CreationTime) &&
                            p.PropertyName != nameof(CreationTimeUTC) &&
                            p.PropertyName != nameof(LastAccessTime) &&
                            p.PropertyName != nameof(LastAccessTimeUTC) &&
                            p.PropertyName != nameof(LastWriteTime) &&
                            p.PropertyName != nameof(LastWriteTimeUTC))
                .ToList();
    }

    protected virtual JsonSerializerSettings GetSettings() => new JsonSerializerSettings()
    {
        ContractResolver = new BaseFileResolver(),
        Formatting = Formatting.Indented
    };

    protected JsonFile(IFile file) : base(file)
    {
        Reload();
    }

    /// <summary>
    /// Will trigger Save() if field has changed.
    /// </summary>
    protected void SetValueIfChangedAndSave<TRet>(ref TRet backingField, TRet newValue)
    {
        if (!EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            backingField = newValue;
            Save();
        }
    }

    public bool Reload()
    {
        try
        {
            var content = this.ReadAllText();
            if (string.IsNullOrEmpty(content))
                this.WriteAllText(content = JsonConvert.SerializeObject(this, GetSettings()));

            JsonConvert.PopulateObject(content, this, GetSettings());
            return true;
        }
        catch (JsonSerializationException) // Json file is invalid, replace with the default valid one.
        {
            // Comment this to prevent replacing invalid json with default values.
            this.WriteAllText(JsonConvert.SerializeObject(this, GetSettings()));
            return false;
        }
    }
    public bool Save()
    {
        try
        {
            this.WriteAllText(JsonConvert.SerializeObject(this, GetSettings()));
            return true;
        }
        catch (JsonSerializationException)
        {
            return false;
        }
    }

    protected async Task<bool> ReloadAsync()
    {
        try
        {
            var content = await this.ReadAllTextAsync();
            if (string.IsNullOrEmpty(content))
                await this.WriteAllTextAsync(content = JsonConvert.SerializeObject(this, GetSettings()));

            JsonConvert.PopulateObject(content, this, GetSettings());
            return true;
        }
        catch (JsonSerializationException) // Json file is invalid, replace with the default valid one.
        {
            // Comment this to prevent replacing with default.
            await this.WriteAllTextAsync(JsonConvert.SerializeObject(this, GetSettings()));
            return false;
        }
    }
    public async Task<bool> SaveAsync()
    {
        try
        {
            await this.WriteAllTextAsync(JsonConvert.SerializeObject(this, GetSettings()));
            return true;
        }
        catch (JsonSerializationException)
        {
            return false;
        }
    }
}
public class SomeJsonFile : JsonFile
{
    private string _serverName = "Put Server Name Here";
    public string ServerName { get => _serverName; set => SetValueIfChangedAndSave(ref _serverName, value); }

    private string _serverMessage = "Put Server Description Here";
    public string ServerMessage { get => _serverMessage; set => SetValueIfChangedAndSave(ref _serverMessage, value); }

    private int _maxPlayers = 1000;
    public int MaxPlayers { get => _maxPlayers; set => SetValueIfChangedAndSave(ref _maxPlayers, value); }


    public SomeJsonFile() : base(new ApplicationRootFolder().CreateFile("Some.json", CreationCollisionOption.OpenIfExists)) { }
}

public class OtherJsonFile : JsonFile
{
    public string ServerName { get; set; } = "Put Server Name Here";

    public string ServerMessage { get; set; } = "Put Server Description Here";

    public int MaxPlayers { get; set; } = 1000;


    public OtherJsonFile() : base(new ApplicationRootFolder().CreateFile("Other.json", CreationCollisionOption.OpenIfExists)) { }
}
```
