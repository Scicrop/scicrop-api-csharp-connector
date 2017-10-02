/* Copyright [2017] [SciCrop INFORMACAO E TECNOLOGIA S.A.]

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciCrop.AgroAPI.Connector.Entities;
using SciCrop.AgroAPI.Connector.Helpers;
using SciCrop.AgroAPI.Connector.Db;

namespace agroapi_csharp_connector
{
    public partial class Form1 : Form
    {
        private AuthEntity authEntity = null;
        private String strDate = null;

        public Form1()
        {
            InitializeComponent();

            try
            {
                string jsonString = FileHelper.Instance.GetStringFromFilePath("agroapi.io.json");
                ScicropEntity scicropEntity = ScicropEntity.FromJson(jsonString);
                authEntity = scicropEntity.AuthEntity;
                label2.Text = authEntity.UserEntity.Email;
                GetLastRun();
            }
            catch (Exception e)
            {
                updateStatus("Error: "+e.Message);
            }
            

            

        }

        private void GetLastRun()
        {

            try
            {
                DbConnector dbc = new DbConnector();
                DateTime collectDate = dbc.GetLatestCall();

                strDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", collectDate);
                ServiceDataGridEntity sd = new ServiceDataGridEntity("FREIGHT", strDate);
                List<ServiceDataGridEntity> sdList = new List<ServiceDataGridEntity>();
                sdList.Add(sd);

                dataGridView1.DataSource = sdList;

                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GetLastRun();
                textBox1.Text = "";
                button1.Enabled = false;
                button1.Text = "Processing...";

                ScicropEntity se = new ScicropEntity();
                PayloadEntity payloadEntity = new PayloadEntity();
                Freight freight = new Freight();
                freight.Date = strDate;
                List<Freight> freightList = new List<Freight>();
                freightList.Add(freight);
                payloadEntity.FreightLst = freightList;
                se.PayloadEntity = payloadEntity;

                string jsonStr = UrlHelper.Instance.PostScicropEntityJsonBA("freight/dailyUpdate", se, authEntity.UserEntity.Email, authEntity.UserEntity.Hash);

                se = ScicropEntity.FromJson(jsonStr);

                freightList = se.PayloadEntity.FreightLst;

                updateStatus("Looking for freight data since: " + strDate);

                if (freightList.Count > 0)
                {
                    updateStatus("Inserting " + freightList.Count + " freight offer(s).");
                    foreach (var item in freightList)
                    {
                        updateStatus(item.Load.LoadName + ": " + item.SourceCity.Name + " > " + item.DestinationCity.Name);
                        DbConnector dbc = new DbConnector();
                        dbc.InsertFreight(item);
                    }
                    updateStatus("All data inserted.");
                }
                else
                {
                    updateStatus("No new freight data was found.");
                }

                GetLastRun();
                
            }
            catch (Exception ex)
            {

                updateStatus("Error: " + ex.Message);
            }
            finally
            {
                button1.Enabled = true;
                button1.Text = "Run";
            }
            

        }

        private void updateStatus(string msg)
        {
            textBox1.Text += msg+"\r\n";
        }
        
    }
}
