using Verse;

namespace OpenWorld
{
    public class ModConfigs : ModSettings
    {
        public bool tradeBool;
        public bool visitBool;
        public bool onlineRaidBool;
        public bool offlineRaidBool;
        public bool spyWarningBool;
        public bool secretBool;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref tradeBool, "tradeBool");
            Scribe_Values.Look(ref visitBool, "visitBool");
            Scribe_Values.Look(ref onlineRaidBool, "onlineRaidBool");
            Scribe_Values.Look(ref offlineRaidBool, "offlineRaidBool");
            Scribe_Values.Look(ref spyWarningBool, "spyWarningBool");
            Scribe_Values.Look(ref secretBool, "secretBool");
            base.ExposeData();

            Main._ParametersCache.inTrade = tradeBool;
            Main._ParametersCache.visitFlag = visitBool;
            Main._ParametersCache.pvpFlag = onlineRaidBool;
            Main._ParametersCache.offlinePvpFlag = offlineRaidBool;
            Main._ParametersCache.spyWarnFlag = spyWarningBool;
            Main._ParametersCache.secretFlag = secretBool;
        }
    }
}
