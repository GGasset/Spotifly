using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Spotifly
{
    internal class SocketHandler
    {
        private int port;
        private string host;
        private int header;

        internal SocketHandler(int port)
        {
            this.port = port;
            this.host = Dns.GetHostName();
            this.header = 64;
        }

        internal Socket ConnectSocket(string server)
        {
            Socket s = null;
            IPHostEntry hostEntry = Dns.GetHostEntry(server);


            foreach (var ip in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(ip, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
            }

            return s;
        }

        internal void SendMessage(string message)
        {
            using (Socket s = ConnectSocket(host))
            {
                if (s == null)
                {
                    return;
                }

                byte[] encodedMessage = Encoding.ASCII.GetBytes(message);

                byte[] encodedLength = Encoding.ASCII.GetBytes(encodedMessage.Length.ToString());

                s.Send(encodedLength);
                s.Send(encodedMessage);
            }
        }
    }
}
