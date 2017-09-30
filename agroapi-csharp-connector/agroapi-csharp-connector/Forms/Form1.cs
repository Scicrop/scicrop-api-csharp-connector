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

        public Form1()
        {
            InitializeComponent();
            string jsonString = FileHelper.Instance.GetStringFromFilePath("agroapi.io.json");
            ScicropEntity scicropEntity = ScicropEntity.FromJson(jsonString);
            authEntity = scicropEntity.AuthEntity;
            label2.Text = authEntity.UserEntity.Email + ":"+authEntity.UserEntity.Hash;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DbConnector dbc = new DbConnector();
            DateTime collectDate = dbc.GetLatestCall();

            String strDate = String.Format("{0:yyyy-MM-dd HH:mm:ss}", collectDate);

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

            foreach (var item in freightList)
            {
                Console.WriteLine(item.DestinationCity.Name);
            }


            

        }

        
    }
}
