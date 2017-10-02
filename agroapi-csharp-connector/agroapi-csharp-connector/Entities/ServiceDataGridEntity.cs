using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SciCrop.AgroAPI.Connector.Entities
{
    public class ServiceDataGridEntity
    {
        public string service { get; set; }
        public string last_run { get; set; }

    public ServiceDataGridEntity(string service, string last_run)
        {
            this.service = service;
            this.last_run = last_run;
        }


    }
}
