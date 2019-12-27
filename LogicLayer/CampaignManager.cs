using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;

namespace LogicLayer
{
    public class CampaignManager : ICampaignManager
    {
        private ICampaignAccessor _campaignAccessor;

        public CampaignManager()
        {
            _campaignAccessor = new CampaignAccessor();
        }

        public List<Campaign> RetrieveCampaignListByUser(int userid)
        {
            try
            {
                return _campaignAccessor.GetAllCampaignsByUserID(userid);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Campaign data not found.", ex);
            }
        }
    }
}
