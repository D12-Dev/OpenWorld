using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public class FactionProductionSite : FactionStructure
    {
        public override Faction holdingFaction => base.holdingFaction;

        public override string structureName => "Production Site";

        public override int structureType => (int)StructureType.ProductionSite;

        public override int structureTile => base.structureTile;

        public FactionProductionSite(Faction holdingFaction, int structureTile)
        {
            this.holdingFaction = holdingFaction;
            this.structureTile = structureTile;
        }
    }
}
