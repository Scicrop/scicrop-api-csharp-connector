using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Configuration;
using SciCrop.AgroAPI.Connector.Entities;

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
                    "           ,[CityDst]" +
                    "           ,[ProvAcroDst]" +
                    "           ,[CollectDate]" +
                    "           ,[CropRefID]" +
                    "           ,[CropName]" +
                    "           ,[CropSrc]" +
                    "           ,[FreightPrice]" +
                    "           ,[TimeInsert])" +
                    "     VALUES" +
                    "           (@0" +
                    "            @1" +
                    "           ,@2" +
                    "           ,@3" +
                    "           ,@4" +
                    "           ,@5" +
                    "           ,@6" +
                    "           ,@7" +
                    "           ,@8" +
                    "           ,@9)";

                    SqlCommand insertCommand = new SqlCommand(sql, conn);

                    insertCommand.Parameters.Add(new SqlParameter("0", freight.SourceCity.Name));
                    insertCommand.Parameters.Add(new SqlParameter("1", freight.SourceCity.ProvinceAcro));
                    insertCommand.Parameters.Add(new SqlParameter("2", freight.DestinationCity.Name));
                    insertCommand.Parameters.Add(new SqlParameter("3", freight.DestinationCity.ProvinceAcro));
                    insertCommand.Parameters.Add(new SqlParameter("4", freight.Date));
                    insertCommand.Parameters.Add(new SqlParameter("5", freight.Load));
                    insertCommand.Parameters.Add(new SqlParameter("6", 1));
                    insertCommand.Parameters.Add(new SqlParameter("7", 1));
                    insertCommand.Parameters.Add(new SqlParameter("8", 1));
                    insertCommand.Parameters.Add(new SqlParameter("9", 1));

                    insertCommand.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
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
                            if (reader.IsDBNull(collectDateIdx)) collectDate = reader.GetDateTime(collectDateIdx);
                        }
                        reader.Close();
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    
                    if(conn != null) conn.Close();
                        
                }
            }

            if (collectDate == DateTime.MinValue) collectDate = DateTime.Now.AddDays(-1);

            return collectDate;
        }
        
    }
}
