using SettingsManager;

namespace SettingsManagerTests
{
    public class TestSettingsModel : SettingsModel
    {
        public virtual int SomeInt { get; set; } = 3;
    }
}
