using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public class FactionWonder : FactionStructure
    {
        public override Faction holdingFaction => base.holdingFaction;

        public override string structureName => "Wonder Structure";

        public override int structureType => (int)StructureType.Wonder;

        public override int structureTile => base.structureTile;

        public FactionWonder(Faction holdingFaction, int structureTile)
        {
            this.holdingFaction = holdingFaction;
            this.structureTile = structureTile;
        }
    }
}
