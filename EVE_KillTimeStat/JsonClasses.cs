using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_KillTimeStat
{
    public class FoundObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string image { get; set; }
    }

    public class SearchObjects
    {
        public List<FoundObject> FoundObjects { get; set; }
    }

    public class Victim
    {
        public int shipTypeID { get; set; }
        public int characterID { get; set; }
        public string characterName { get; set; }
        public int corporationID { get; set; }
        public string corporationName { get; set; }
        public int allianceID { get; set; }
        public string allianceName { get; set; }
        public int factionID { get; set; }
        public string factionName { get; set; }
        public int damageTaken { get; set; }
    }

    public class Attacker
    {
        public int characterID { get; set; }
        public string characterName { get; set; }
        public int corporationID { get; set; }
        public string corporationName { get; set; }
        public int allianceID { get; set; }
        public string allianceName { get; set; }
        public int factionID { get; set; }
        public string factionName { get; set; }
        public double securityStatus { get; set; }
        public int damageDone { get; set; }
        public int finalBlow { get; set; }
        public int weaponTypeID { get; set; }
        public int shipTypeID { get; set; }
    }

    public class Position
    {
        public double y { get; set; }
        public double x { get; set; }
        public double z { get; set; }
    }

    public class Zkb
    {
        public int locationID { get; set; }
        public string hash { get; set; }
        public double fittedValue { get; set; }
        public double totalValue { get; set; }
        public int points { get; set; }
        public bool npc { get; set; }
    }

    public class Kill
    {
        public int killID { get; set; }
        public int solarSystemID { get; set; }
        public string killTime { get; set; }
        public int moonID { get; set; }
        public Victim victim { get; set; }
        public List<Attacker> attackers { get; set; }
        public List<object> items { get; set; }
        public Position position { get; set; }
        public Zkb zkb { get; set; }
    }

    public class PlayerKills
    {
        public List<Kill> Kills { get; set; }
    }
}
