using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Whatsapplike
{

    public class Cliente
    {
        private int puerto;
        private TcpClient? tcpClient;
        private NetworkStream? networkStream;

        public Cliente(int puertoCliente)
        {
            puerto = puertoCliente;
        }

        public void Conectar(string address)
        {
            try
            {
                tcpClient = new TcpClient(address, puerto);
                networkStream = tcpClient.GetStream();
                Console.WriteLine("Conectado al servidor.");

                Thread recibirThread = new Thread(RecibirMensaje);
                recibirThread.IsBackground = true;
                recibirThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
            }
        }

        public void RecibirMensaje()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesLeidos = networkStream?.Read(buffer, 0, buffer.Length) ?? 0;
                    if (bytesLeidos > 0)
                    {
                        string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
                        Console.WriteLine("Mensaje recibido: " + mensaje);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void EnviarMensaje(string mensaje)
        {
            if (tcpClient?.Connected == true)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(mensaje);
                networkStream?.Write(buffer, 0, buffer.Length);
            }
        }

        public void Desconectar()
        {
            tcpClient?.Close();
        }
    }

}
