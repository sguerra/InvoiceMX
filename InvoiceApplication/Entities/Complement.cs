using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class Complement
    {
        #region Properties

        [XmlElement("TimbreFiscalDigital", typeof(DigitalTaxStamp), Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
        public DigitalTaxStamp DigitalTaxStamp { get; set; }

        #endregion
    }
}
