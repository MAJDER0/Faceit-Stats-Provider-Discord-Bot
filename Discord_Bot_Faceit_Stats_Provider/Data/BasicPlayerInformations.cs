﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Faceit_Stats_Provider.Data
{
    public class BasicPlayerInformations
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

        public class Csgo
        {
            public string region { get; set; }
            public string game_player_id { get; set; }
            public int skill_level { get; set; }
            public int faceit_elo { get; set; }
            public string game_player_name { get; set; }
            public string skill_level_label { get; set; }
            public Regions regions { get; set; }
            public string game_profile_id { get; set; }
        }

        public class Games
        {
            public Csgo csgo { get; set; }
        }

        public class Infractions
        {
        }

        public class Platforms
        {
            public string steam { get; set; }
        }

        public class Regions
        {
        }

        public class Settings
        {
            public string language { get; set; }
        }
    }
}
