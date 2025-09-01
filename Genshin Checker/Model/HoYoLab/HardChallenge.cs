using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Checker.Model.HoYoLab.HardChallenge
{
    public class Root : Model.HoYoLab.Root<Data>
    {
    }
    public class Schedule
    {
        public string schedule_id { get; set; } = "";
        public string start_time { get; set; } = "";
        public string end_time { get; set; } = "";
        public DateTimeClass? start_date_time { get; set; }
        public DateTimeClass? end_date_time { get; set; }
        public bool is_valid { get; set; }
        public string name { get; set; } = "";
    }
    public class DateTimeClass
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
    }
    public class Best
    {
        public int difficulty { get; set; }
        public int second { get; set; }
        public string icon { get; set; } = "";
    }

    public class BestAvatar
    {
        public int avatar_id { get; set; }
        public string side_icon { get; set; } = "";
        public string dps { get; set; }
        public int type { get; set; }
    }

    public class Challenge
    {
        public string name { get; set; } = "";
        public int second { get; set; }
        public List<Team> teams { get; set; }
        public List<BestAvatar> best_avatar { get; set; } = new();
        public Monster monster { get; set; } 
    }

    public class Datum
    {
        public Schedule schedule { get; set; }
        /// <summary>
        /// シングルプレイヤー
        /// </summary>
        public BattleData single { get; set; }
        /// <summary>
        /// マルチプレイヤー
        /// </summary>
        public BattleData mp { get; set; }
        public List<Bling> blings { get; set; }
    }

    public class Bling
    {
        public int avatar_id { get; set; }
        public string name { get; set; } = "";
        public string element { get; set; } = "none";
        public string image { get; set; } = "";
        public bool is_plus { get; set; }
        public int rarity { get; set; }
        public string side_icon { get; set; } = "";
    }

    public class Links
    {
        public string lineup_link { get; set; }
        public string play_link { get; set; }
    }

    public class Monster
    {
        public string name { get; set; }
        public int level { get; set; }
        public string icon { get; set; }
        public List<string> desc { get; set; }
        public List<Tag> tags { get; set; }
        public int monster_id { get; set; }
    }

    public class Data
    {
        public List<Datum> data { get; set; }
        public bool is_unlock { get; set; }
        public Links links { get; set; }
    }


    public class BattleData
    {
        public Best? best { get; set; }
        public List<Challenge> challenge { get; set; }
        public bool has_data { get; set; }
    }


    public class Tag
    {
        public int type { get; set; }
        public string desc { get; set; }
    }

    public class Team
    {
        public int avatar_id { get; set; }
        public string name { get; set; }
        public string element { get; set; }
        public string image { get; set; }
        public int level { get; set; }
        public int rarity { get; set; }
        public int rank { get; set; }
    }

}
