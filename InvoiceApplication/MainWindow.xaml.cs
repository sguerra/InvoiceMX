using InvoiceApplication.Entities;
using InvoiceApplication.Properties;
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

        private string DefaultDirectory
        {
            get 
            {
                return Settings.Default.DEFAULT_DIRECTORY;
            }
        }
        private string InputDirectory
        {
            get 
            {
                return Settings.Default.INPUT_DIRECTORY;
            }
            set
            {
                Settings.Default.INPUT_DIRECTORY = value;
                Settings.Default.Save();
            }
        }
        private string OutputDirectory
        {
            get
            {
                return Path.Combine(this.InputDirectory, "output");
            }
        }
        
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
            if (this.InputDirectory.Equals(this.DefaultDirectory)){
                System.Windows.MessageBox.Show("Bienvenido, selecciona un directorio con tus CFDIs", "InvoiceMX");
                return;
            }

            string dirPath = this.InputDirectory;

            try
            {
                this.LoadDirectory(dirPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "InvoiceMX");
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
            this.InputDirectory = dirPath;
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
            string outputPath = this.OutputDirectory;

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
        private void BtnPolicy_Click(object sender, RoutedEventArgs e)
        {
            string templateFile = "template.txt";
            string outputFile = "policy.csv";

            string outputPath = this.OutputDirectory;
            string templateDirectory = Path.Combine(Directory.GetCurrentDirectory(), templateFile);

            // Open template file
            if (!File.Exists(templateDirectory))
            {
                System.Windows.MessageBox.Show(string.Format("No se encontro la plantilla de poliza"), "InvoiceMX");
                return;
            }

            // Load values into template
            string templateString = File.ReadAllText(templateDirectory);
            string contentValue = string.Empty;

            StringBuilder content = new StringBuilder();

            foreach (Invoice invoice in this.Invoices)
            {
                content.AppendFormat("{0},{1},{2},{3},{4},\r\n", string.Empty, invoice.Issuer.Name.Replace(",", string.Empty), invoice.Subtotal, string.Empty, invoice.UUID, string.Empty);
                content.AppendFormat("{0},{1},{2},{3},{4},\r\n", string.Empty, string.Empty, invoice.Taxes.Total, string.Empty, string.Empty, string.Empty);
                content.AppendFormat("{0},{1},{2},{3},{4},\r\n", string.Empty, string.Empty, string.Empty, invoice.Total, string.Empty, string.Empty);
                content.AppendLine();
            }

            // Save output file 
            contentValue = templateString.Replace("$content", content.ToString());
            
            string filePath = Path.Combine(outputPath, outputFile);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllText(filePath, contentValue);
            Process.Start(filePath);
        }


        #endregion

    }
}
