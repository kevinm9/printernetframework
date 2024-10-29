using System;
using System.Reflection;
namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //EPSON L3250 Series
            Console.WriteLine("Simulador Mejorado de Servicio de Impresión");
            var service = new ServicePrinter();
            while (true)
            {
                Console.WriteLine("\nOpciones:");
                Console.WriteLine("1. Mostrar impresoras disponibles");
                Console.WriteLine("2. Seleccionar impresora");
                Console.WriteLine("3. Establecer ruta de archivo");
                Console.WriteLine("4. Imprimir archivo");
                Console.WriteLine("5. Iniciar servicio de impresión automática");
                Console.WriteLine("6. Detener servicio de impresión automática");
                Console.WriteLine("7. Salir");
                Console.WriteLine("8. test de imprimir por kevin");
                Console.Write("Seleccione una opción: ");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        service.ShowPrinters();
                        break;
                    case "2":
                        Console.Write("Ingrese el nombre de la impresora: ");
                        service.SelectPrinter(Console.ReadLine());
                        service.SelectPrinter("EPSON L3250 Series");
                        break;
                    case "3":
                        Console.Write("Ingrese la ruta del archivo: ");
                        service.SetFilePath(Console.ReadLine());
                        break;
                    case "4":
                        if (!string.IsNullOrEmpty(service.filePath))
                        {
                            service.PrintFile(service.filePath);
                        }
                        else
                        {
                            Console.WriteLine("Primero debe establecer la ruta del archivo.");
                        }
                        break;
                    case "5":
                        service.Start();
                        break;
                    case "6":
                        service.Stop();
                        break;
                    case "7":
                        service.Stop();
                        return;
                    case "8":
                        string printer = "EPSON L3250 Series";
                        string paperName = "A4";
                        string filename = @"C:\current\prueba.pdf";
                        int copies = 1;
                        bool result = service.PrintPDF(printer, paperName, filename, copies);
                        if (result)
                        {
                            Console.WriteLine("PDF impreso con éxito.");
                        }
                        else
                        {
                            Console.WriteLine("Error al imprimir el PDF.");
                        }
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
        
        }
    }
}