using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // for mainnet set host to "ws.mainnet.digitexfutures.com"
            string host = "ws.testnet.digitexfutures.com";
            bool wss = true;
            string token = "d030b17c9b993107399eb19121daea0f04e4b476";

            IntervalAlgorithm algorithm = new IntervalAlgorithm(host, token, wss);
            algorithm.Prepare();
            algorithm.Connect();

            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }
    }
}
