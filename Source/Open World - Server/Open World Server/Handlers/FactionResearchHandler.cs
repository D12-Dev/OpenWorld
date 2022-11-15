using System.Collections.Generic;
using System.Linq;

namespace OpenWorldServer
{
    public static class FactionResearchHandler
    {
        public static void HandleFactionResearchSettings(ServerClient client)
        {
            Networking.SendData(client, "FactionResearch│ResearchSettings│" + Server.usingSharedFactionResearch + "│" + Server.forcedResearchMultiplier);
        }

        public static void SetCurrentProgress(ServerClient client, string techName, float techProgress, float techCost)
        {
            if (client.faction == null) return;

            FactionResearch factionResearch = getFactionResearch(client.faction);

            Dictionary<string, Dictionary<string, float>> allResearchProgresses = factionResearch.researchProgresses;
            Dictionary<string, float> allResearchCosts = factionResearch.allResearchCosts;

            // Validate the cost
            if (!allResearchCosts.ContainsKey(techName))
            {
                allResearchCosts.Add(techName, techCost);
            }

            else if (!MathUtils.Approximately(allResearchCosts[techName], techCost))
            {
                ConsoleUtils.LogToConsole("ERROR in FactionResearchHandler, tech cost mismatch for [" + techName + "]: [" + allResearchCosts[techName] + "] - [" + techCost + "]", ConsoleUtils.ConsoleLogMode.Error);
                allResearchCosts[techName] = techCost; // Overwrite the value with the new value anyway 
            }

            // Update the progress
            Dictionary<string, float> researchProgress;
            allResearchProgresses.TryGetValue(techName, out researchProgress);
            if (researchProgress == null)
            {
                researchProgress = new Dictionary<string, float>();
                allResearchProgresses.Add(techName, researchProgress);
            }

            researchProgress[client.username] = techProgress;

            // Check for completion
            float totalResearchProgress = researchProgress.Sum(x => x.Value);
            if (totalResearchProgress >= allResearchCosts[techName])
            {
                if (!factionResearch.completedResearch.Contains(techName))
                {
                    factionResearch.completedResearch.Add(techName);
                }
                allResearchProgresses.Remove(techName);

                FactionHandler.SaveFaction(client.faction);

                // Send a message to all faction members who are online
                ServerClient[] factionMembers = client.faction.members.Keys.ToArray();
                foreach (ServerClient factionMember in factionMembers)
                {
                    ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == factionMember.username);
                    if (connected != null)
                    {
                        sendResearchCompleteMessages(connected, techName);
                    }
                }
            }
        }

        public static void RequestFullResearchReport(ServerClient client)
        {
            Networking.SendData(client, "FactionResearch│RequestFullResearchReport│");
        }

        public static void HandleFullResearchReport(ServerClient client, ISet<string> completedTechs, Dictionary<string, float> techProgresses, Dictionary<string, float> techCosts)
        {
            if (client.faction == null) return;
            FactionResearch factionResearch = getFactionResearch(client.faction);

            if (factionResearch == null || completedTechs == null || techProgresses == null || techCosts == null)
            {
                ConsoleUtils.LogToConsole("ERROR in FactionResearchHandler, some input was null: [" + factionResearch + "] - [" + completedTechs + "] - [" + techProgresses + "] - [" + techCosts + "]", ConsoleUtils.ConsoleLogMode.Error);
                return;
            }

            ISet<string> completedResearches = factionResearch.completedResearch;
            Dictionary<string, Dictionary<string, float>> allResearchProgresses = factionResearch.researchProgresses;
            Dictionary<string, float> allResearchCosts = factionResearch.allResearchCosts;

            List<string> missingCompletedTechs = new List<string>();
            List<string> newlyCompletedTechs = new List<string>();

            // Check for missing completed techs
            foreach (string completedResearch in completedResearches)
            {
                if (!completedTechs.Contains(completedResearch))
                {
                    missingCompletedTechs.Add(completedResearch);
                }
            }

            // Check for newly completed techs
            foreach (string completedTech in completedTechs)
            {
                if (!completedResearches.Contains(completedTech))
                {
                    completedResearches.Add(completedTech);
                    newlyCompletedTechs.Add(completedTech);
                    allResearchProgresses.Remove(completedTech);
                }
            }

            // Set and validate research costs
            foreach (KeyValuePair<string, float> techCost in techCosts)
            {
                if (!allResearchCosts.ContainsKey(techCost.Key))
                {
                    allResearchCosts.Add(techCost.Key, techCost.Value);
                }

                else if (!MathUtils.Approximately(allResearchCosts[techCost.Key], techCost.Value))
                {
                    ConsoleUtils.LogToConsole("ERROR in FactionResearchHandler, tech cost mismatch for [" + techCost.Key + "]: [" + allResearchCosts[techCost.Key] + "] - [" + techCost.Value + "]", ConsoleUtils.ConsoleLogMode.Error);
                    allResearchCosts[techCost.Key] = techCost.Value; // Overwrite the value with the new value anyway 
                }
            }

            // Set in progress values and check for completed techs
            foreach (KeyValuePair<string, float> techProgress in techProgresses)
            {
                Dictionary<string, float> reseachProgress;
                allResearchProgresses.TryGetValue(techProgress.Key, out reseachProgress);
                if (reseachProgress == null)
                {
                    reseachProgress = new Dictionary<string, float>();
                    allResearchProgresses.Add(techProgress.Key, reseachProgress);
                }
                reseachProgress[client.username] = techProgress.Value;
                float totalResearchProgress = reseachProgress.Sum(x => x.Value);
                if (totalResearchProgress >= allResearchCosts[techProgress.Key])
                {
                    if (!completedResearches.Contains(techProgress.Key))
                    {
                        completedResearches.Add(techProgress.Key);
                    }
                    newlyCompletedTechs.Add(techProgress.Key);
                    allResearchProgresses.Remove(techProgress.Key);
                }
            }

            FactionHandler.SaveFaction(client.faction);

            if (missingCompletedTechs.Count > 0)
            {
                sendResearchCompleteMessages(client, missingCompletedTechs);
            }

            ServerClient[] factionMembers = client.faction.members.Keys.ToArray();
            foreach (ServerClient factionMember in factionMembers)
            {
                if (factionMember.username != client.username)
                {
                    ServerClient connected = Networking.connectedClients.Find(fetch => fetch.username == factionMember.username);
                    if (connected != null)
                    {
                        sendResearchCompleteMessages(connected, newlyCompletedTechs);
                    }
                }
            }
        }

        private static FactionResearch getFactionResearch(Faction faction)
        {
            FactionResearch factionResearch = faction.factionResearch;
            if (factionResearch == null)
            {
                factionResearch = new FactionResearch();
                faction.factionResearch = factionResearch;
            }
            return factionResearch;
        }

        private static void sendResearchCompleteMessages(ServerClient client, List<string> completedResearches)
        {
            sendResearchCompleteMessages(client, string.Join(",", completedResearches));
        }

        private static void sendResearchCompleteMessages(ServerClient client, string completedResearches)
        {
            Networking.SendData(client, "FactionResearch│CompleteResearches│" + completedResearches);
        }
    }
}
