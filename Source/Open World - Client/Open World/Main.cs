using System.Globalization;
using Verse;

namespace OpenWorld
{
    public class Main
    {
        public static ModConfigs _modConfigs = new ModConfigs();
        public static ParametersCache _ParametersCache = new ParametersCache();
        public static Injections _Injections = new Injections();
        public static Encryption _Encryption = new Encryption();
        public static MPCaravan _MPCaravan = new MPCaravan();
        public static MPWorld _MPWorld = new MPWorld();
        public static MPGame _MPGame = new MPGame();
        public static MPChat _MPChat = new MPChat();
        public static Networking _Networking = new Networking();
        public static Threading _Threading = new Threading();

        [StaticConstructorOnStartup]
        public static class OpenWorld
        {
            static OpenWorld()
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
            }
        }
    }
}
