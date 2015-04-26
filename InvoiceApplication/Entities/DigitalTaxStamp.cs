using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class DigitalTaxStamp
    {
        #region Properties

        [XmlAttribute("UUID")]
        public string UUID { get; set; }

        #endregion

    }
}
