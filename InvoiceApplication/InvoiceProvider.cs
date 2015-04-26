using InvoiceApplication.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace InvoiceApplication
{
    internal class InvoiceProvider
    {
        #region Fields

        private static InvoiceProvider invoiceProvider;

        #endregion
        #region Constructor

        private InvoiceProvider ()
	    {
	    }
        public static InvoiceProvider GetInstance() 
        {
            if (invoiceProvider == null)
                invoiceProvider = new InvoiceProvider();

            return invoiceProvider;
        }

        #endregion
        #region Methods

        public Invoice GetInvoice(string xmlPath)
        {
            Invoice invoice = null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Invoice), "cfdi");
                TextReader reader = new StreamReader(xmlPath);

                invoice = (Invoice)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {

            }


            return invoice;
        }

        #endregion
    }
}
