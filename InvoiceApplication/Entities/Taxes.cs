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

        [XmlArray("Retenciones", Namespace = "http://www.sat.gob.mx/cfd/3")]
        [XmlArrayItem("Retencion", typeof(TaxRetention), Namespace = "http://www.sat.gob.mx/cfd/3")]
        public List<TaxRetention> Retentions { get; set; }

        #endregion
    }
}
