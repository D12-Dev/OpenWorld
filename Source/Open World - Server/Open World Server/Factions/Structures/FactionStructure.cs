using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    [System.Serializable]
    public abstract class FactionStructure
    {
        public enum StructureType { Silo, Marketplace, ProductionSite, Wonder, Bank, Stable, Courier }

        public virtual Faction holdingFaction { get; set; }

        public virtual string structureName { get; set; }

        public virtual int structureType { get; set; }

        public virtual int structureTile { get; set; }
    }
}