using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Drawing.Printing;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleAppserver
{
    class Program
    {
        static int requestCount = 0;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_HIDE = 0;

        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8000/");
            listener.Start();
            Console.WriteLine("App iniciada");
            Console.WriteLine("Servidor escuchando en http://localhost:8000/");

            Thread serverThread = new Thread(() => HandleRequests(listener));
            serverThread.Start();
        }

        static void HandleRequests(HttpListener listener)
        {
            while (true)
            {
                try
                {
                    // Escuchar la petición entrante
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Añadir encabezados CORS
                    response.Headers.Add("Access-Control-Allow-Origin", "*"); // Permitir todas las solicitudes
                    response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                    // Si es una solicitud OPTIONS, termina aquí
                    if (request.HttpMethod == "OPTIONS")
                    {
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.Close();
                        continue; // Salta el resto del procesamiento
                    }

                    // Incrementar el contador de peticiones y mostrar en consola
                    requestCount++;
                    Console.WriteLine($"Petición número {requestCount} recibida: {request.HttpMethod} {request.Url.AbsolutePath}");

                    // Procesar la solicitud GET para impresoras
                    if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/printers")
                    {
                        var printers = GetInstalledPrinters();
                        var printerStatusList = ValidatePrinters(printers);

                        var jsonResponse = new
                        {
                            status = "ok",
                            data = new { printers, printerStatusList },
                            message = "List of installed printers retrieved successfully"
                        };

                        SendJsonResponse(response, 200, jsonResponse);
                    }
                    else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/print")
                    {
                        try
                        {
                            using (var reader = new StreamReader(request.InputStream))
                            {
                                string body = reader.ReadToEnd();

                                // Asegúrate de que el cuerpo de la solicitud no sea nulo ni vacío
                                if (string.IsNullOrWhiteSpace(body))
                                {
                                    var errorResponse = new
                                    {
                                        status = "error",
                                        data = (object)null,
                                        message = "Request body is empty or null"
                                    };
                                    SendJsonResponse(response, 400, errorResponse);
                                    continue; // Continua con la siguiente petición
                                }

                                // Deserialización del cuerpo de la solicitud
                                var printRequest = JsonConvert.DeserializeObject<PrintRequest>(body);

                                if (printRequest == null)
                                {
                                    var errorResponse = new
                                    {
                                        status = "error",
                                        data = (object)null,
                                        message = "Invalid JSON format"
                                    };
                                    SendJsonResponse(response, 400, errorResponse);
                                    continue; // Continua con la siguiente petición
                                }

                                // Validación del contenido deserializado
                                var validationResult = ValidatePrintRequest(printRequest);
                                if (!validationResult.isValid)
                                {
                                    var errorResponse = new
                                    {
                                        status = "error",
                                        data = (object)null,
                                        message = validationResult.errorMessage
                                    };
                                    SendJsonResponse(response, 400, errorResponse);
                                }
                                else
                                {
                                    var printers = GetInstalledPrinters();
                                    var successResponse = new
                                    {
                                        printer = printRequest.Printer,
                                        paperName = printRequest.PaperName,
                                        filename = printRequest.Filename,
                                        copies = printRequest.Copies,
                                        printers // Lista de impresoras instaladas
                                    };

                                    SendJsonResponse(response, 200, successResponse);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorResponse = new
                            {
                                status = "error",
                                data = (object)null,
                                message = $"Error processing the request: {ex.Message}"
                            };
                            SendJsonResponse(response, 500, errorResponse);
                        }
                    }
                    else
                    {
                        var errorResponse = new
                        {
                            status = "error",
                            data = (object)null,
                            message = "Route not found"
                        };
                        SendJsonResponse(response, 404, errorResponse);
                    }
                }
                catch (HttpListenerException ex)
                {
                    Console.WriteLine("HttpListenerException: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unhandled exception: " + ex.Message);
                    // Muestra el stack trace para poder depurar el error
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        // Función para enviar la respuesta JSON
        static void SendJsonResponse(HttpListenerResponse response, int statusCode, object responseObject)
        {
            try
            {
                if (response == null || response.OutputStream == null)
                {
                    return; // Si la respuesta o el flujo de salida ya están cerrados, no hacer nada
                }

                string jsonResponse = JsonConvert.SerializeObject(responseObject);

                response.StatusCode = statusCode;
                response.ContentType = "application/json";
                response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);
                using (StreamWriter writer = new StreamWriter(response.OutputStream))
                {
                    writer.Write(jsonResponse);
                }
                response.Close(); // Asegúrate de cerrar la respuesta
            }
            catch (ObjectDisposedException)
            {
                // Maneja el caso donde el objeto ya ha sido desechado
                Console.WriteLine("El objeto HttpListenerResponse ya ha sido cerrado.");
            }
        }



        // Función que obtiene la lista de impresoras instaladas
        static List<string> GetInstalledPrinters()
        {
            List<string> printerList = new List<string>();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printerList.Add(printer);
            }

            return printerList;
        }

        // Función que valida si las impresoras están disponibles
        static List<string> ValidatePrinters(List<string> printers)
        {
            List<string> printerStatusList = new List<string>();

            foreach (var printer in printers)
            {
                try
                {
                    using (var printDoc = new PrintDocument())
                    {
                        printDoc.PrinterSettings.PrinterName = printer;

                        // Verificar si la impresora está lista
                        if (printDoc.PrinterSettings.IsValid)
                        {
                            printerStatusList.Add($"{printer}: Ready");
                        }
                        else
                        {
                            printerStatusList.Add($"{printer}: Not Ready");
                        }
                    }
                }
                catch (Exception ex)
                {
                    printerStatusList.Add($"{printer}: Error - {ex.Message}");
                }
            }

            return printerStatusList;
        }

        // Validación del JSON recibido
        static (bool isValid, string errorMessage) ValidatePrintRequest(PrintRequest printRequest)
        {
            if (string.IsNullOrWhiteSpace(printRequest.Printer))
                return (false, "Printer is required");

            if (string.IsNullOrWhiteSpace(printRequest.PaperName))
                return (false, "PaperName is required");

            if (string.IsNullOrWhiteSpace(printRequest.Filename) || !File.Exists(printRequest.Filename))
                return (false, "Filename is either empty or does not exist");

            if (printRequest.Copies <= 0)
                return (false, "Copies must be greater than 0");

            return (true, null);
        }

        // Clase para mapear el JSON de la solicitud de impresión
        public class PrintRequest
        {
            [JsonProperty("printer")]
            public string Printer { get; set; }

            [JsonProperty("paperName")]
            public string PaperName { get; set; }

            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("copies")]
            public int Copies { get; set; }
        }

    }
}
