namespace Discord_Bot_Faceit_Stats_Provider.Data
{
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
}
