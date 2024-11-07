Console Print Server Activated from Frontend
This repository contains a console-based printing application developed in Visual Studio using .NET Framework. The application operates as a lightweight print server, allowing direct printing from the console in response to requests sent from a frontend. It includes an endpoint to list available printers and another to send print commands to the specified printer.

Features
Console-Based Print Server: Runs as a console application, receiving and processing print requests from the frontend.
Printer Listing: Provides an endpoint to display available printers.
Direct Printing: Allows frontend-triggered print execution on the server’s console.
Customizable Settings: Supports configurable print parameters through frontend requests.
Main Endpoints
/printers - Returns a list of connected and available printers.
/print - Receives a print request and executes the print process on the specified printer.
Requirements
Visual Studio: Developed and configured in Visual Studio.
.NET Framework: Uses .NET Framework for backend logic and print management.
System Print Access: The console application requires access to the system printers for listing and printing.
Installation and Setup
Clone this repository and open the project in Visual Studio.
Configure the appsettings.json file (if necessary) for printer connection settings.
Compile and run the console server. Ensure the frontend is set up to send requests to the server’s endpoints.
Execution
Run the server in the console, making sure the frontend is connected and able to communicate with the endpoints. From the frontend, you can send requests to list available printers and trigger printing on the selected printer.

Main Branch
The main branch of this project is main.


***************************************
Problema: Se encontró una lógica con impresoras de tickets, pero es muy engorroso para el cliente seguir los pasos en esta solución. En esta nueva solución, el cliente solo ejecuta un archivo .exe que actúa como un servidor que escuchará al frontend, el cual no está en local, sino en internet. Sin embargo, se conectará a los endpoints locales, ya que la aplicación ejecutada es un pequeño servidor que escucha la petición que haga el frontend. Ambos proyectos están separados, pero solo hay que conectarlos. La idea es que el frontend se pueda comunicar con el cliente que está en local. Mediante esta conexión, se accede a la impresora; allí están las aplicaciones que, como servidor, escuchan al frontend. Luego, por debajo, se puede consultar la impresora que tiene el cliente y devolver un JSON. La función para imprimir está separada; solo la unes y le envías los parámetros, ya que el endpoint ya está creado.

                Console.WriteLine("8. test de imprimir por kevin");



*************************************************

Características
Servidor de Impresión en Consola: La aplicación se ejecuta en consola y actúa como servidor de impresión, recibiendo solicitudes desde el frontend.
Listado de Impresoras: Un endpoint que permite visualizar las impresoras disponibles en el sistema.
Impresión Directa: Ejecuta solicitudes de impresión desde el frontend mediante la consola del servidor.
Configuración Personalizable: La aplicación permite configurar la impresora y los parámetros de impresión a través de los datos enviados desde el frontend.
Endpoints Principales
/printers - Devuelve un listado de las impresoras conectadas y disponibles.
/print - Recibe una solicitud de impresión y ejecuta el proceso en la impresora especificada.
Requisitos
Visual Studio: Proyecto configurado y desarrollado en Visual Studio.
.NET Framework: La aplicación utiliza .NET Framework para la lógica de backend y la gestión de impresiones.
Acceso al Sistema de Impresión: La consola debe tener acceso a las impresoras del sistema para listar y ejecutar las impresiones.
Instalación y Configuración
Clona este repositorio y abre el proyecto en Visual Studio.
Configura el archivo appsettings.json (si es necesario) para ajustar los datos de conexión a las impresoras.
Compila y ejecuta el servidor de consola. Asegúrate de que el frontend esté configurado para enviar solicitudes a los endpoints del servidor.
Ejecución
Inicia el servidor en consola, asegurándote de que el frontend esté conectado y pueda comunicarse con los endpoints. Desde el frontend, puedes enviar solicitudes para listar las impresoras disponibles y ejecutar la impresión en la impresora seleccionada.

Rama Principal
La rama principal del proyecto es main.
