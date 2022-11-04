using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public class FactionBank : FactionStructure
    {
        public override Faction holdingFaction => base.holdingFaction;

        public override string structureName => "Bank";

        public override int structureType => (int)StructureType.Bank;

        public override int structureTile => base.structureTile;

        public int depositedSilver;

        public FactionBank(Faction holdingFaction, int structureTile)
        {
            this.holdingFaction = holdingFaction;
            this.structureTile = structureTile;
        }
    }
}
