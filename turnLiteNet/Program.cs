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
            Client[] clients = new Client[10];
            for(int i=0; i< clients.Length; ++i)
            {
                clients[i] = new Client();
                Console.WriteLine(clients[i].ToString());
            }

            s.Setup();
            foreach (Client c in clients)
            {
                c.Connect();
            }


            //infinite update loop
            while (true)
            {
                s.Update();
                foreach(Client c in clients) 
                {
                    c.Update();
                }

            }
        }
    }
}
