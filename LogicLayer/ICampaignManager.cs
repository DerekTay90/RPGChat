using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface ICampaignManager
    {
        List<Campaign> RetrieveCampaignListByUser(int userid);
    }
}
