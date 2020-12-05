using System;
using DigitexConnector.Enums;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            DigitexConnector.Configuration.Server = Servers.testnet;
            DigitexConnector.Configuration.Token = "<your_API_token>";

            IntervalAlgorithm algorithm = new IntervalAlgorithm();
            algorithm.Prepare();
            algorithm.Connect();

            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }
    }
}
