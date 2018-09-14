using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettingsManager;
using SettingsManager.ModelProcessors;

namespace SettingsManagerTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var settings = 
                new SettingsBuilder<TestSettingsModel>()
                    .WithProcessor(new JsonModelProcessor())
                    .WithFile(@"G:\Files\Temp\settings.json")
                    .Build();

            settings.SomeInt = 7;

            
        }
    }
}
