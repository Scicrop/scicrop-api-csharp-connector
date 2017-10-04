using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;
using SciCrop.AgroAPI.Connector.Entities;
using System.Globalization;

namespace SciCrop.AgroAPI.Connector.Db
{
    public class DbConnector
    {

        public void InsertFreight(Freight freight)
        {
            string connStr = null;
            connStr = ConfigurationManager.ConnectionStrings["SciCrop.AgroAPI.Connector.Properties.Settings.dbConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sql = "INSERT INTO [dbo].[SCICROP_FREIGHT_T]" +
                    "           ([CitySrc]" +
                    "           ,[ProvAcroSrc]" +
                    "           ,[CitySrcIbge]" +
                    "           ,[CityDst]" +
                    "           ,[ProvAcroDst]" +
                    "           ,[CityDestIbge]" +
                    "           ,[CollectDate]" +
                    "           ,[CropName]" +
                    "           ,[CropSrc]" +
                    "           ,[FreightPrice]" +
                    "           ,[FreightDist])" + 
                    "     VALUES" +
                    "           (@0" +
                    "           ,@1" +
                    "           ,@2" +
                    "           ,@3" +
                    "           ,@4" +
                    "           ,@5" +
                    "           ,@6" +
                    "           ,@7" +
                    "           ,@8" +
                    "           ,@9" +
                    "           ,@10)";

                    SqlCommand insertCommand = new SqlCommand(sql, conn);

                    insertCommand.Parameters.Add(new SqlParameter("0", freight.SourceCity.Name));
                    insertCommand.Parameters.Add(new SqlParameter("1", freight.SourceCity.ProvinceAcro));
                    insertCommand.Parameters.Add(new SqlParameter("2", freight.SourceCity.IbgeCode));
                    insertCommand.Parameters.Add(new SqlParameter("3", freight.DestinationCity.Name));
                    insertCommand.Parameters.Add(new SqlParameter("4", freight.DestinationCity.ProvinceAcro));
                    insertCommand.Parameters.Add(new SqlParameter("5", freight.DestinationCity.IbgeCode));
                    
                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(freight.Date));

                    //DateTime.ParseExact(freight.Date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    
                    insertCommand.Parameters.Add(new SqlParameter("6", dt));
                    insertCommand.Parameters.Add(new SqlParameter("7", freight.Load.LoadName));
                    insertCommand.Parameters.Add(new SqlParameter("8", freight.Load.LoadRaw));
                    insertCommand.Parameters.Add(new SqlParameter("9", freight.Price));
                    insertCommand.Parameters.Add(new SqlParameter("10", freight.Km));

                    insertCommand.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {

                    if (conn != null) conn.Close();

                }
            }
        }

      public DateTime GetLatestCall()
        {
            DateTime collectDate = DateTime.MinValue;

            string connStr = null;
            connStr = ConfigurationManager.ConnectionStrings["SciCrop.AgroAPI.Connector.Properties.Settings.dbConnectionString"].ConnectionString;
            
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("SELECT TOP 1 CollectDate FROM [dbo].[SCICROP_FREIGHT_T] ORDER BY CollectDate DESC", conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int collectDateIdx = reader.GetOrdinal("CollectDate");
                            if (!reader.IsDBNull(collectDateIdx)) collectDate = reader.GetDateTime(collectDateIdx);
                           
                        }
                        reader.Close();
                    }
                    
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    
                    if(conn != null) conn.Close();
                        
                }
            }

            return collectDate;
        }
        
    }
}
