using System;
using System.Drawing.Printing;
using System.Linq;
using System.IO;
using System.Threading;
using PdfiumViewer;
namespace ConsoleApp3
{
    internal class ServicePrinter
    {
        private Timer _timer;
        private string selectedPrinter;
        private bool isRunning = false;
        public string filePath;
        public ServicePrinter()
        {
        }
        public string[] getAllPrinters()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
        }
        public void Start()
        {
            if (!isRunning)
            {
                Console.WriteLine("Iniciando servicio de impresion...");
                _timer = new Timer(AutoPrintJob, null, 0, 60000);
            }
        }
        public void Stop()
        {
            if (isRunning)
            {
                Console.WriteLine("Deteniendo servicio de impresion...");
                _timer.Dispose();
                isRunning = false;
            }
        }
        private void AutoPrintJob(object state)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Console.WriteLine($"Imprimiendo archivo: {filePath}");
                PrintFile(filePath);
            }
        }
        public void ShowPrinters()
        {
            Console.WriteLine("Impresoras disponibles:");
            string[] printers = this.getAllPrinters();
            foreach (string printerName in printers)
            {
                Console.WriteLine(printerName);
            }
        }
        public void SelectPrinter(string printerName)
        {
            var printerSettigs = this.getAllPrinters();
            if (!printerSettigs.Contains(printerName))
            {
                Console.WriteLine("Impresora no encontrada.");
            }
            else
            {
                selectedPrinter = printerName;
                Console.WriteLine($"Impresora seleccionada: {selectedPrinter}");
            }
        }
        public void SetFilePath(string path)
        {
            if (File.Exists(path))
            {
                filePath = path;
                Console.WriteLine($"Archivo seleccionado: {filePath}");
            }
            else
            {
                Console.WriteLine("Archivo no encontrado.");
            }
        }
        public void PrintFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Archivo no encontrado.");
                return;
            }
            try
            {
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrinterSettings.PrinterName = selectedPrinter;
                    pd.PrintController = new StandardPrintController();
                    pd.Print();
                }
                Console.WriteLine("Documento enviado a imprimir.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al imprimir: {ex.Message}");
            }
        }

        public bool PrintPDF(string printer,string paperName,string filename,int copies)
        {
            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printer,
                    Copies = (short)copies,
                };

                // Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0),
                };
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName == paperName)
                    {
                        pageSettings.PaperSize = paperSize;
                        break;
                    }
                }

                // Now print the PDF document
                using (var document = PdfDocument.Load(filename))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }

}