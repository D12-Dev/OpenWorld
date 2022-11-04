using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public class FactionCourierStation : FactionStructure
    {
        public override Faction holdingFaction => base.holdingFaction;

        public override string structureName => "Courier Station";

        public override int structureType => (int)StructureType.Courier;

        public override int structureTile => base.structureTile;

        public FactionCourierStation(Faction holdingFaction, int structureTile)
        {
            this.holdingFaction = holdingFaction;
            this.structureTile = structureTile;
        }
    }
}
