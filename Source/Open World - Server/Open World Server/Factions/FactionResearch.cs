using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{

    [System.Serializable]
    public class FactionResearch
    {
        public ISet<string> completedResearch = new HashSet<string>();
        public Dictionary<string, Dictionary<string, float>> researchProgresses = new Dictionary<string, Dictionary<string, float>>();
        public Dictionary<string, float> allResearchCosts = new Dictionary<string, float>();
    }
}