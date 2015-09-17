using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class TaxRetention
    {
        #region Properties

        [XmlAttribute("importe")]
        public double Total { get; set; }

        [XmlAttribute("impuesto")]
        public string Tax { get; set; }

        #endregion
    }
}
