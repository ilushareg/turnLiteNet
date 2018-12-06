using System;
using LiteNetLib;

namespace turnLiteNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Server s = new Server();
            Client[] clients = new Client[2];
            for(int i=0; i< clients.Length; ++i)
            {
                clients[i] = new Client();
                Console.WriteLine(clients[i].ToString());
            }


            //infinite update loop
            while (!Console.KeyAvailable)
            {
                s.Update();
                foreach(Client c in clients) 
                {
                    c.Update();
                }

                System.Threading.Thread.Sleep(500);
                Console.Write('.');
            }
        }
    }
}
