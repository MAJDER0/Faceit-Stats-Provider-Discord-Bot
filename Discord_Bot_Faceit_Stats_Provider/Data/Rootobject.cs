using System.Runtime;
using System.Security.Cryptography;
using System;

namespace Discord_Bot_Faceit_Stats_Provider.Data
{
    public class Rootobject
    {
        public string player_id { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public string country { get; set; }
        public string cover_image { get; set; }
        public Platforms platforms { get; set; }
        public Games games { get; set; }
        public Settings settings { get; set; }
        public string[] friends_ids { get; set; }
        public string new_steam_id { get; set; }
        public string steam_id_64 { get; set; }
        public string steam_nickname { get; set; }
        public string[] memberships { get; set; }
        public string faceit_url { get; set; }
        public string membership_type { get; set; }
        public string cover_featured_image { get; set; }
        public Infractions infractions { get; set; }
    }
}
