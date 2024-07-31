using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Whatsapplike
{
    public class Servidor
    {
        private int puerto;
        private TcpListener escucha;
        private ConcurrentDictionary<TcpClient, NetworkStream> clientes;

        public Servidor(int puertoServidor)
        {
            puerto = puertoServidor;
            escucha = new TcpListener(IPAddress.Any, puerto);
            clientes = new ConcurrentDictionary<TcpClient, NetworkStream>();
        }

        public void Inicio()
        {
            escucha.Start();
            Console.WriteLine("Servidor iniciado en el puerto: " + puerto);

            Thread aceptar = new Thread(AceptarClientes);
            aceptar.IsBackground = true;
            aceptar.Start();
        }

        public void AceptarClientes()
        {
            while (true)
            {
                var cliente = escucha.AcceptTcpClient();
                var stream = cliente.GetStream();
                clientes.TryAdd(cliente, stream);

                Thread hiloClientes = new Thread(() => ManejarCliente(cliente, stream));
                hiloClientes.IsBackground = true;
                hiloClientes.Start();
            }
        }

        private void ManejarCliente(TcpClient cliente, NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    if (bytesLeidos > 0)
                    {
                        string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
                        Console.WriteLine("Mensaje recibido: " + mensaje);
                        EnviarMensaje(mensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                clientes.TryRemove(cliente, out _);
            }
        }

        private void EnviarMensaje(string mensaje)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(mensaje);

            foreach (var cliente in clientes.Keys)
            {
                if (cliente.Connected)
                {
                    var stream = clientes[cliente];
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public void Terminar()
        {
            escucha.Stop();
        }
    }

}
