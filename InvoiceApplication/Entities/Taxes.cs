using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class Taxes
    {
        #region Properties

        [XmlAttribute("totalImpuestosTrasladados")]
        public double Total { get; set; }

        #endregion
    }
}
