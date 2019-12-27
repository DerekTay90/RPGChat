using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface ICampaignAccessor
    {
        List<Campaign> GetAllCampaignsByUserID(int UserID);
    }
}
