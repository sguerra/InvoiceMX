using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    public class Issuer
    {
        #region Properties

        [XmlAttribute("nombre")]
        public string Name { get; set; }
        [XmlAttribute("rfc")]
        public string RFC { get; set; }

        #endregion
    }
}
