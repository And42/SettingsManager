using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SettingsManager
{
    /// <summary>
    /// Class for managing settings. Supports caching.
    /// </summary>
    public abstract class SettingsContainerBase : ISettingsContainer
    {
        public virtual bool AutoFlush { get; set; } = true;

        private readonly Dictionary<string, object> _cachedProperties = new Dictionary<string, object>();

        public T GetValue<T>(string settingName)
        {
            return GetValueInternal<T>(settingName);
        }

        public void SetValue<T>(string settingName, T value)
        {
            SetValueInternal(value, settingName);
        }

        public object GetValue(string settingName)
        {
            return GetValueInternal(settingName);
        }

        public void SetValue(string settingName, object value)
        {
            SetValueInternal(value, settingName);
        }

        public abstract void Save();

        protected abstract void SetSetting(string settingName, object value);

        protected abstract object GetSetting(string settingName);

        protected object GetValueInternal([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (_cachedProperties.TryGetValue(propertyName, out object value))
                return value;

            object val = GetSetting(propertyName);

            _cachedProperties.Add(propertyName, val);

            return val;
        }

        protected T GetValueInternal<T>([CallerMemberName] string propertyName = null)
        {
            return (T)GetValueInternal(propertyName);
        }

        protected void SetValueInternal<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (_cachedProperties.TryGetValue(propertyName, out var cachedValue))
            {
                if (EqualityComparer<T>.Default.Equals((T)cachedValue, value))
                    return;

                _cachedProperties[propertyName] = value;
            }
            else
            {
                _cachedProperties.Add(propertyName, value);
            }

            SetSetting(propertyName, value);

            if (AutoFlush)
                Save();

            OnPropertyChanged(propertyName);
        }

        protected void SetValueInternal(object value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (_cachedProperties.TryGetValue(propertyName, out var cachedValue))
            {
                if (Equals(cachedValue, value))
                    return;

                _cachedProperties[propertyName] = value;
            }
            else
            {
                _cachedProperties.Add(propertyName, value);
            }

            SetSetting(propertyName, value);

            if (AutoFlush)
                Save();

            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
