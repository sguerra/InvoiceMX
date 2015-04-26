using InvoiceApplication.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace InvoiceApplication
{   
    public class InvoiceList : List<Invoice>
    {
        #region Fields

        private Dictionary<string, Func<Invoice, object>> sortPredicates { get; set; }
 
        #endregion
        #region Constructor

        public InvoiceList()
        {
            this.sortPredicates = new Dictionary<string, Func<Invoice, object>>();

            this.sortPredicates.Add("RFC", (x => x.Issuer.RFC));
            this.sortPredicates.Add("FECHA", (x => x.Date));
        }

        #endregion
        #region Methods

        public List<Invoice> sortBy(string text, bool ascending)
        {
            if(!this.sortPredicates.ContainsKey(text))
                return this;

            Func<Invoice, object> predicate = this.sortPredicates[text];

            if (!ascending)
                return this.OrderByDescending(predicate).ToList();

            return this.OrderBy(predicate).ToList();
        }

        #endregion
    }

    public partial class MainWindow : Window
    {
        #region Fields

        private InvoiceProvider invoiceProvider;

        #endregion
        #region Properties

        private InvoiceList Invoices { get; set; }

        #endregion
        #region Construtor
        public MainWindow()
        {
            InitializeComponent();
            invoiceProvider = InvoiceProvider.GetInstance();
        }

        #endregion
        #region Methods

        private void LoadDirectory(string dirPath) 
        {
            this.Invoices = new InvoiceList();
            string[] fileNames = Directory.GetFiles(dirPath);

            foreach (string fileName in fileNames)
            {
                string extension = Path.GetExtension(fileName);

                if (!extension.Equals(".XML", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                Invoice invoice = invoiceProvider.GetInvoice(fileName);
                Invoices.Add(invoice);
            }

            this.LbxInvoices.ItemsSource = this.Invoices;
        }

        #endregion
        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //return;
            string dirPath = "\\\\VBOXSVR\\VB_shared_folder\\2014\\DICIEMBRE\\egreso";

            try
            {
                this.LoadDirectory(dirPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Hay un problema");
            }
        }
        private void BtnLoadDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            string dirPath = dialog.SelectedPath;

            if (dirPath.Equals(string.Empty))
                return;

            this.LoadDirectory(dirPath);
        }
        private void LbxInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Invoice invoice = this.LbxInvoices.SelectedItem as Invoice;

            if (invoice == null)
                return;

            //this.LbxItems.ItemsSource = invoice.Items;
        }
        
        private void BtnCheckRepeated_Click(object sender, RoutedEventArgs e)
        {
            if (this.Invoices == null)
                return;

            List<Invoice> repeated = new List<Invoice>();

            foreach (Invoice item in this.Invoices)
            {
                if (repeated.Count(x => x.UUID.Equals(item.UUID)) != 0)
                    continue;

                if (this.Invoices.Count(x => x.UUID.Equals(item.UUID)) > 1)
                    repeated.Add(item);
            }

            if (repeated.Count == 0)
                System.Windows.MessageBox.Show("No existen comprobantes repetidos", "Revisión completa", MessageBoxButton.OK, MessageBoxImage.None);
            else if (repeated.Count == 1)
                System.Windows.MessageBox.Show(string.Format("Existe un comprobantes repetido:\n{0}", repeated.First().UUID), "Revisión completa", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                System.Windows.MessageBox.Show(string.Format("Existen comprobantes repetidos:\n{0}", string.Join("\n", repeated.Select(x => x.UUID).ToArray())), "Revisión completa", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SortBy(object sender, RoutedEventArgs e)
        {
            TextBlock tsender = (TextBlock)sender;
            List<Invoice> orderedInvoices = this.Invoices.sortBy(tsender.Text, true);

            this.LbxInvoices.ItemsSource = orderedInvoices;
        }


        private void BtnSaveDirectory_Click(object sender, RoutedEventArgs e)
        {
            string outputPath = "\\\\VBOXSVR\\VB_shared_folder\\2014\\DICIEMBRE\\output";

            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            foreach(Invoice selectedItem in  this.LbxInvoices.SelectedItems)
            {
                string fileName = Path.GetFileName(selectedItem.FilePath);
                string copyPath = Path.Combine(outputPath, fileName);

                string dirPath = Path.GetDirectoryName(selectedItem.FilePath);
                string searchPattern = fileName.Replace(".xml", "*.pdf");

                List<string> files = Directory.GetFiles(dirPath, searchPattern).ToList();
                string pdfPath = files.FirstOrDefault();
                
                if(pdfPath!= null)
                {
                    string pdfName = Path.GetFileName(pdfPath);
                    string pdfCopyPath = Path.Combine(outputPath, pdfName);
                    File.Copy(pdfPath, pdfCopyPath);
                }

                File.Copy(selectedItem.FilePath, copyPath);
            }

            Process.Start(outputPath);
        }

        #endregion
    }
}
