using System.Globalization;
using Verse;

namespace OpenWorld
{
    public static class Main
    {
        public static ParametersCache _ParametersCache = new ParametersCache();
        public static Injections _Injections = new Injections();
        public static ModConfigs _modConfigs = new ModConfigs();
        public static MPWorld _MPWorld = new MPWorld();
        public static MPGame _MPGame = new MPGame();
        public static MPChat _MPChat = new MPChat();

        [StaticConstructorOnStartup]
        public static class OpenWorld
        {
            static OpenWorld()
            {
                SetupCulture();
            }

            private static void SetupCulture()
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                CultureInfo.CurrentUICulture = new CultureInfo("en-US", false);
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
            }
        }
    }
}
