using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Campaign
    {
        public int CampaignID { get; set; }
        public User DungeonMaster { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
