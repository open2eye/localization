using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Checker.Model.UserData.HardChallenge.v1
{
    public class V1 : DatabaseRoot
    {
        public Data Data { get; set; } = new();
    }
    public class Schedule
    {
        /// <summary>
        /// スケジュールID
        /// </summary>
        public int schedule_id { get; set; }
        /// <summary>
        /// 開始時刻
        /// </summary>
        public DateTime start_time { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 終了時刻
        /// </summary>
        public DateTime end_time { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// スケジュール名(言語データ含む)
        /// </summary>
        public string name { get; set; } = "";
    }
    public class Best
    {
        public int difficulty { get; set; } = 0;
        public int second { get; set; } = 0;
        public string icon { get; set; } = "";
    }
        
    public class BestAvatar
    {
        public int avatar_id { get; set; }
        public int damages { get; set; }
        public int type { get; set; }
    }

    public class Challenge
    {
        /// <summary>
        /// モンスター情報の配列
        /// </summary>
        public Monster monster { get; set; } = new();

        public string name { get; set; } = "";
        public int second { get; set; }
        public List<Team> teams { get; set; } = new();
        public List<BestAvatar> best_avatar { get; set; } = new();
    }

    public class Data
    {
        public Schedule schedule { get; set; } = new();
        /// <summary>
        /// シングルプレイヤー
        /// </summary>
        public List<BattleData> single { get; set; } = new();
        /// <summary>
        /// マルチプレイヤー
        /// </summary>
        public List<BattleData> mp { get; set; } = new();
        //public List<Monster> monsters { get; set; } = new();
        public List<Bling> blings { get; set; } = new();
    }
    public class Bling
    {
        public int avater_id { get; set; }
        public bool is_plus { get; set; }
    }
    public class Monster
    {
        public string name { get; set; } = "";
        public int level { get; set; }
        public string icon { get; set; } = "";
        public List<string> desc { get; set; } = new();
        public List<Tag> tags { get; set; } = new();
        public int monster_id { get; set; }
    }



    public class BattleData
    {
        public DateTime created_at { get; set; } = DateTime.MinValue;
        public Best best { get; set; } = new();
        public List<Challenge> challenge { get; set; } = new();
    }


    public class Tag
    {
        public int type { get; set; }
        public string desc { get; set; } = "";
    }

    public class Team
    {
        public int avatar_id { get; set; }
        public int level { get; set; }
        public int rank { get; set; }
    }
}
