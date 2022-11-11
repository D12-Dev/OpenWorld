using Verse;

namespace OpenWorld
{
    /**
     * Using a GameComponent with ExposeData so that this value is saved with the Game in case the Game is loaded up offline.  This will be per Game and not impact other Games the user may load.
     */
    public class FactionResearchGameComponent : GameComponent
    {
        public static float researchCostMultiplier = 1.0f;
        public static bool factionResearchEnabled = false;

        public FactionResearchGameComponent(Game game) : this()
        {
            // This constructor is needed despite not being in the GameComponent contract
        }

        public FactionResearchGameComponent()
        {
            // Reset values on new game load, they will be update by the server and/or the game 
            researchCostMultiplier = 1.0f;
            factionResearchEnabled = false;
        }

        public override void ExposeData()
        {
            // Scribe is magic, it internally knows if it's loading or saving these values
            Scribe_Values.Look(ref researchCostMultiplier, "researchCostMultiplier", 1.0f);
            Scribe_Values.Look(ref factionResearchEnabled, "factionResearchEnabled", false);
        }
    }
}
