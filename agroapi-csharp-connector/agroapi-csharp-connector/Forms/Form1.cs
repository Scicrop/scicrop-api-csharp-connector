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
using System.Diagnostics;
using System.Globalization;

namespace agroapi_csharp_connector
{
    public partial class Form1 : Form
    {
        private AuthEntity authEntity = null;
        private String strDateFreight = null;
        private String strDateVeg = null;
        private DateTime collectDateFreight = DateTime.MinValue;
        private DateTime collectDateVeg = DateTime.MinValue;
        private bool isSilent = false;

        public Form1(bool isSilent)
        {
            this.isSilent = isSilent;
            InitializeComponent();

            try
            {

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;

                this.Text +=" ["+version+"]";

                string jsonString = FileHelper.Instance.GetStringFromFilePath("agroapi.io.json");
                ScicropEntity scicropEntity = ScicropEntity.FromJson(jsonString);
                authEntity = scicropEntity.AuthEntity;
                label2.Text = authEntity.UserEntity.Email;
                GetLastRun();
                if (isSilent) Run();
            }
            catch (Exception e)
            {
                updateStatus("Error: "+e.Message);
            }
            

            

        }

        private void WriteEventLog(string message)
        {
            try
            {
                string sSource = "SciCrop AgroAPI Connector";
                string sLog = "Application";

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);
                
                EventLog.WriteEntry(sSource, message, EventLogEntryType.Information);
                Console.WriteLine(message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetLastRun()
        {

            try
            {
                List<ServiceDataGridEntity> sdList = new List<ServiceDataGridEntity>();

                DbConnector dbc = new DbConnector();

                collectDateFreight = dbc.GetFreightLatestCall();
                strDateFreight = String.Format("{0:yyyy-MM-dd HH:mm:ss}", collectDateFreight);
                ServiceDataGridEntity sd = new ServiceDataGridEntity("FREIGHT", strDateFreight);
                sdList.Add(sd);

                collectDateVeg = dbc.GetVegLatestCall();
                strDateVeg = String.Format("{0:yyyy-MM-dd HH:mm:ss}", collectDateVeg);
                sd = new ServiceDataGridEntity("VEGETABLES", strDateVeg);
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

            Run();

        }

        private void Run()
        {
            try
            {
                progressBar1.Value = 0;
                GetLastRun();
                string restFreight = "freight/dailyUpdate";
                string restVegetables = "vegetables/dailyUpdateQuotation/"+ collectDateVeg.ToString("yyyy-MM-dd");
                if (collectDateFreight == DateTime.MinValue)
                {
                    restFreight = "freight/historic";
                    updateStatus("Downloading freight historical data. ");
                }
                else
                {
                    updateStatus("Looking for freight data since: " + strDateFreight);
                }

                if (collectDateVeg == DateTime.MinValue)
                {
                    restVegetables = "vegetables/historicQuotation";
                    updateStatus("Downloading vegetables historical data. ");
                }
                else
                {
                    updateStatus("Looking for vegetables data since: " + strDateVeg);
                }


                //textBox1.Text = "";
                button1.Text = "Processing...";
                button1.Enabled = false;


                ScicropEntity se = new ScicropEntity();
                PayloadEntity payloadEntity = new PayloadEntity();

                Freight freight = new Freight();
                freight.Date = strDateFreight;
                List<Freight> freightList = new List<Freight>();
                freightList.Add(freight);
                payloadEntity.FreightLst = freightList;

                VegetablesQuotationEntity vegetablesQuotationEntity = new VegetablesQuotationEntity();
                vegetablesQuotationEntity.PriceDate = strDateVeg;
                List<VegetablesQuotationEntity> vegs = new List<VegetablesQuotationEntity>();
                vegs.Add(vegetablesQuotationEntity);
                payloadEntity.VegetablesQuotationEntities = vegs;

                se.PayloadEntity = payloadEntity;

                

                string jsonStr = UrlHelper.Instance.PostScicropEntityJsonBA(restFreight, se, authEntity.UserEntity.Email, authEntity.UserEntity.Hash);
                se = ScicropEntity.FromJson(jsonStr);
                freightList = se.PayloadEntity.FreightLst;


                if (freightList.Count > 0)
                {
                    int i = 1;
                    updateStatus("Inserting " + freightList.Count + " freight offer(s).");
                    foreach (var item in freightList)
                    {

                        updateStatus(item.Load.LoadName + ": " + item.SourceCity.Name + " > " + item.DestinationCity.Name);
                        DbConnector dbc = new DbConnector();
                        try
                        {
                            dbc.InsertFreight(item);
                        }
                        catch (Exception e)
                        {
                            updateStatus(e.Message);
                        }
                        progressBar1.Maximum = freightList.Count;
                        progressBar1.Value = i;
                        i++;
                    }
                    
                    updateStatus("All freight data inserted.");
                }
                else
                {
                    updateStatus("No new freight data was found.");
                }
                WriteEventLog("Freight data collected (" + freightList.Count + " | REST: " + restFreight + " | "+isSilent+")");
                GetLastRun();

                jsonStr = UrlHelper.Instance.GetScicropEntityJsonBA(restVegetables, authEntity.UserEntity.Email, authEntity.UserEntity.Hash);
                se = ScicropEntity.FromJson(jsonStr);
                vegs = se.PayloadEntity.VegetablesQuotationEntities;

                if (vegs.Count > 0)
                {
                    int i = 1;
                    updateStatus("Inserting " + vegs.Count + " vegetables quotation(s).");
                    foreach (var item in vegs)
                    {

                        updateStatus(item.VegName + ": " + item.VegPrice + " from  " + item.IbgeUfId);
                        DbConnector dbc = new DbConnector();
                        try
                        {
                            dbc.InsertVegetableQuotation(item);
                        }
                        catch (Exception e)
                        {
                            updateStatus(e.Message);
                        }
                        progressBar1.Maximum = vegs.Count;
                        progressBar1.Value = i;
                        i++;
                    }

                    updateStatus("All Vegetables data inserted.");
                }
                else
                {
                    updateStatus("No new Vegetables data was found.");
                }
                WriteEventLog("Vegetables data collected (" + vegs.Count + " | REST: " + restVegetables + " | " + isSilent + ")");
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
                if (this.isSilent) System.Environment.Exit(0);
            }
        }

        private void updateStatus(string msg)
        {
            textBox1.AppendText(msg + "\r\n");
        }
        
    }
}
