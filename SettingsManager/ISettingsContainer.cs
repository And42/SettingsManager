using System.ComponentModel;

namespace SettingsManager
{
    public interface ISettingsContainer : INotifyPropertyChanged
    {
        bool AutoFlush { get; set; }

        T GetValue<T>(string settingName);

        void SetValue<T>(string settingName, T value);

        object GetValue(string settingName);

        void SetValue(string settingName, object value);

        void Save();
    }
}