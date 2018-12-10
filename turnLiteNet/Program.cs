using System;
using LiteNetLib;

namespace turnLiteNet
{
    class Program
    {

        static void UpdateLoop(int seconds, Server s, Client []clients)
        {
            for(int i=0; i< seconds*2; ++i) 
            {
                s.Update();
                foreach (Client c in clients)
                {
                    if (c != null)
                    {
                        c.Update();
                    }
                }

                System.Threading.Thread.Sleep(500);
                Console.Write('.');

            }

        }

        static void Main(string[] args)
        {

            //if(2 == 1)
            //{
            //    byte [] test = new byte[] { (byte)10};
            //    int tst = test[0];

            //    return;
            //}

            Console.WriteLine("Hello World!");

            Server s = new Server();
            Client[] clients = new Client[3];

            for(int i=0; i< clients.Length; ++i)
            {
                clients[i] = new Client();
                Console.WriteLine(clients[i].ToString());
            }


            //infinite update loop
            UpdateLoop(200, s, clients);

        }
    }
}
