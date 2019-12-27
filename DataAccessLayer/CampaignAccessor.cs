using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class CampaignAccessor : ICampaignAccessor
    {
        public List<Campaign> GetAllCampaignsByUserID(int UserID)
        {
            List<Campaign> campaigns = new List<Campaign>();

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_select_campaings_by_user_id");

            cmd.Connection = conn;

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", UserID);


            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var campaign = new Campaign();
                        campaign.CampaignID = reader.GetInt32(0);
                        campaign.Name = reader.GetString(2);
                        campaign.Description = reader.GetString(3);
                        campaigns.Add(campaign);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return campaigns;
        }
    }
}
