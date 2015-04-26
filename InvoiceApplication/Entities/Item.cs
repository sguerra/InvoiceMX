using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class Item
    {
        #region Properties

        [XmlAttribute("importe")]
        public double Total { get; set; }

        [XmlAttribute("cantidad")]
        public double Quantity{get; set;}

        [XmlAttribute("unidad")]
        public string Unity{get; set;}
        
        [XmlAttribute("descripcion")]
        public string Description{get; set;}
        
        [XmlAttribute("valorUnitario")]
        public double UnitPrice{get; set;}
          
        #endregion
    }
}
