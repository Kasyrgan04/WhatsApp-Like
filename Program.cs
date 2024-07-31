using System;
using Whatsapplike;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length > 0 && (args[0] == "--servidor" || args[0] == "--cliente"))
        {
            if (args[0] == "--servidor")
            {
                if (args.Length == 3 && args[1] == "-port" && int.TryParse(args[2], out int puertoServidor))
                {
                    Servidor servidor = new Servidor(puertoServidor);
                    servidor.Inicio();

                    Console.WriteLine("Presione ENTER para salir...");
                    Console.ReadLine();
                    servidor.Terminar();
                }
                else
                {
                    Console.WriteLine("Uso correcto: --servidor -port <puerto>");
                }
            }
            else if (args[0] == "--cliente")
            {
                if (args.Length == 4 && args[1] == "-port" && int.TryParse(args[2], out int puertoCliente) && !string.IsNullOrEmpty(args[3]))
                {
                    Cliente cliente = new Cliente(puertoCliente);
                    cliente.Conectar(args[3]);

                    string mensaje;
                    while ((mensaje = Console.ReadLine()) != null)
                    {
                        cliente.EnviarMensaje(mensaje);
                    }
                }
                else
                {
                    Console.WriteLine("Uso correcto: --cliente -port <puerto> <direccion>");
                }
            }
        }
        else
        {
            Console.WriteLine("Especificar '--servidor' o '--cliente' como argumento");
        }
    }
}
