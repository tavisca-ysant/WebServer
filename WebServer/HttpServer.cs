using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebServer
{
   public class HttpServer
    {
        public const String Message_Directory = "/bin/Debug/netcoreapp2.2/root/Message/";
        public const String Web_Directory = "/bin/Debug/netcoreapp2.2/root/web";

        public const String Version = "HTTP/1.1";
        public const String Name = "C# Http Server";
        private bool _running = false;
        private TcpListener _tcpListener;

        public HttpServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {

            _running = true;
            _tcpListener.Start();
            while (_running)
            {
                Console.WriteLine("Waiting for connection");
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Client connected");
                HandleClient(tcpClient);

                tcpClient.Close();
            }
            _running = false;
            _tcpListener.Stop();

        }

        private void HandleClient(TcpClient tcpClient)
        {
            StreamReader streamReader = new StreamReader(tcpClient.GetStream());

            String message = "";
            while((streamReader.Peek() != -1))
            {
                message += streamReader.ReadLine() + "\n";
            }
            Debug.WriteLine("Request: \n" + message);

            Request request = Request.GetRequest(message);
            Response response = Response.From(request);
            response.Post(tcpClient.GetStream());
        }
    }
}
