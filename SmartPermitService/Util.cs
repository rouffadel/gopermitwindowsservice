using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPermitService
{
    public class Util
    {
        public static string StrConnection;
        static Util()
        {
            StrConnection = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }
        public static DataSet GetVehicleDetails(ArrayList arrayList)
        {
            try
            {
                Common.InfoLogs("GetVehicleDetails Method Started, Class:Util");

                DataSet ds = SqlHelper.ExecuteDataset(StrConnection, "Sp_GetZatParkVehicles", arrayList.ToArray());
                return ds;
            }
            catch (Exception ex)
            {
                Common.InfoLogs("Error occured at GetVehicleDetails Method, Class:Util" + ex.Message.ToString());
                return null;
            }
        }

        public static DataSet GetSites()
        {
            try
            {
                Common.InfoLogs("GetVisitorVehicleDetails Method Started, Class:Util");

                DataSet ds = SqlHelper.ExecuteDataset(StrConnection, "Sp_GetSites");
                return ds;
            }
            catch (Exception ex)
            {
                Common.InfoLogs("Error occured at GetVisitorVehicleDetails Method, Class:Util" + ex.Message.ToString());
                return null;
            }
        }


        public static DataSet GetVisitorVehicleDetails(ArrayList array)
        {
            try
            {
                Common.InfoLogs("GetVisitorVehicleDetails Method Started, Class:Util, connection="+StrConnection);

                DataSet ds = SqlHelper.ExecuteDataset(StrConnection, "Sp_GetVisitorParkVehicles",array.ToArray());
                return ds;
            }
            catch (Exception ex)
            {
                Common.InfoLogs("Error occured at GetVisitorVehicleDetails Method, Class:Util" + ex.Message.ToString());
                return null;
            }
        }

        public static DataSet UpdateVehicleDetails(ArrayList array)
        {
            try
            {
                Common.InfoLogs("UpdateCampaignStatus Method Started, Class:Util");

                DataSet ds = SqlHelper.ExecuteDataset(StrConnection, "Sp_UpdateZatparkVehicle", array.ToArray());
                return ds;
            }
            catch (Exception ex)
            {
                Common.InfoLogs("Error occured at UpdateVehicleDetails Method, Class:Util" + ex.Message.ToString());
                return null;
            }
        }

        public static DataSet UpdateVisitorVehicleDetails(ArrayList array)
        {
            try
            {
                Common.InfoLogs("UpdateCampaignStatus for Visitor Method Started, Class:Util");

                DataSet ds = SqlHelper.ExecuteDataset(StrConnection, "Sp_UpdateVisitorparkVehicle", array.ToArray());
                return ds;
            }
            catch (Exception ex)
            {
                Common.InfoLogs("Error occured at UpdateVisitorVehicleDetails Method, Class:Util" + ex.Message.ToString());
                return null;
            }
        }
    }
}
