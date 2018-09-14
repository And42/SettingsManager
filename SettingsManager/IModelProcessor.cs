namespace SettingsManager
{
    public interface IModelProcessor
    {
        T LoadModelFromString<T>(string input) where T : SettingsModel;

        string SaveModelToString<T>(T model) where T : SettingsModel;
    }
}