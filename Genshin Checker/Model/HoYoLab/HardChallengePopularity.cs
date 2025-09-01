using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Checker.Model.HoYoLab.HardChallengePopularity
{
    public class Root : Model.HoYoLab.Root<Data>
    {
    }
    public class  Data
    {
        public List<Avatar> avatar_list { get; set; } = new();
    }
    public class Avatar
    {
        public int avatar_id { get; set; }
        public string name { get; set; } = "";
        public string element { get; set; } = "";
        public string image { get; set; } = "";
        public int rarity { get; set; }
    }

}
