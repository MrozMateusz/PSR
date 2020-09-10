using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt4
{
    class NetCom
    {
        private SimpleTcpServer server = null;
        private SimpleTcpClient client = null;
        private Logic logic;
        private int portE = 80;

        private SimpleTCP.Message activeMessage = null;

        internal void init()
        {
            for(int i = 0; i < 10; i++)
            {
                try
                {
                    this.Setup(80 + i);
                    portE += i;
                }
                catch
                {

                }
            }
        }

        public NetCom(Logic logic)
        {
            this.logic = logic;
        }


        public void Setup(int port)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13; //enter
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
            System.Net.IPAddress ip = System.Net.IPAddress.Parse("127.0.0.1");
            server.Start(ip, port);

           
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
            
        }

        private void ConnectClient(int port)
        {
            client.Connect(("127.0.0.1"), port);
        }
        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            ConnectClient(portE);
            String response = logic.ServerJob(e);
            this.activeMessage.ReplyLine(response);
        }
        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            logic.ClientJob(e);
        }
        internal void SendNewRequest(String message)
        {
              client.WriteLineAndGetReply(message, TimeSpan.FromSeconds(1));
        }
    }
}

