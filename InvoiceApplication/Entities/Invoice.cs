using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication.Entities
{
    [XmlRoot("Comprobante", Namespace = "http://www.sat.gob.mx/cfd/3")]
    public class Invoice
    {
        #region Properties

        [XmlElement("Emisor", typeof(Issuer), Namespace = "http://www.sat.gob.mx/cfd/3")]
        public Issuer Issuer { get; set; }
        public string UUID
        {
            get
            {
                return this.Complement.DigitalTaxStamp.UUID.ToUpper();
            }
        }

        [XmlAttribute("folio")]
        public int Folio {get; set; }

        [XmlAttribute("fecha")]
        public DateTime Date {get; set; }

        [XmlAttribute("subTotal")]
        public double Subtotal { get; set; }
        [XmlAttribute("total")]
        public double Total { get; set; }

        [XmlArray("Conceptos", Namespace = "http://www.sat.gob.mx/cfd/3")]
        [XmlArrayItem("Concepto", typeof(Item), Namespace = "http://www.sat.gob.mx/cfd/3")]
        public List<Item> Items { get; set; }

        [XmlElement("Impuestos", typeof(Taxes), Namespace = "http://www.sat.gob.mx/cfd/3")]
        public Taxes Taxes { get; set; }

        [XmlElement("Complemento", typeof(Complement), Namespace = "http://www.sat.gob.mx/cfd/3")]
        public Complement Complement { get; set; }

        public string FilePath { get; set; }

        #endregion
    }
}
