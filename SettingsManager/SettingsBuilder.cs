using System;
using System.IO;
using System.Text;
using Castle.DynamicProxy;
using LongPaths.Logic;

// ReSharper disable JoinNullCheckWithUsage

namespace SettingsManager
{
    public class SettingsBuilder<T> where T : SettingsModel, new()
    {
        private class SettingsInterceptor : IInterceptor
        {
            private readonly IModelProcessor _modelProcessor;
            private readonly string _filePath;

            public SettingsInterceptor(
                IModelProcessor modelProcessor,
                string filePath
            )
            {
                _modelProcessor = modelProcessor;
                _filePath = filePath;
            }

            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();

                string methodName = invocation.Method.Name;

                if (methodName.StartsWith("set_", StringComparison.Ordinal))
                {
                    T model = (T) invocation.Proxy;
                    model.OnPropertyChanged(methodName.Substring(4));
                    SaveModel(model);
                }
            }

            private void SaveModel(T model)
            {
                if (string.IsNullOrEmpty(_filePath))
                    return;

                CheckSettingsFileDir();
                LFile.WriteAllText(_filePath, _modelProcessor.SaveModelToString(model), Encoding.UTF8);
            }

            private void CheckSettingsFileDir()
            {
                string directory = Path.GetDirectoryName(_filePath);

                if (directory != null && !LDirectory.Exists(directory))
                    LDirectory.CreateDirectory(directory);
            }
        }

        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        private IModelProcessor _modelProcessor;
        private string _filePath;

        public SettingsBuilder<T> WithProcessor(IModelProcessor modelProcessor)
        {
            _modelProcessor = modelProcessor;
            return this;
        }

        public SettingsBuilder<T> WithFile(string filePath)
        {
            _filePath = filePath;
            return this;
        }

        public T Build()
        {
            T initialModel = LoadInitialModel();

            T proxy = 
                ProxyGenerator.CreateClassProxyWithTarget(
                    initialModel, 
                    new SettingsInterceptor(
                        _modelProcessor,
                        _filePath
                    )
                );

            return proxy;
        }

        private T LoadInitialModel()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                if (_modelProcessor == null)
                {
                    throw new ArgumentException(
                        $"Can't use \"{nameof(_filePath)}\" when \"{nameof(_modelProcessor)}\" is not set"
                    );
                }

                if (LFile.Exists(_filePath))
                    return _modelProcessor.LoadModelFromString<T>(LFile.ReadAllText(_filePath, Encoding.UTF8));

                return new T();
            }

            return new T();
        }
    }
}
