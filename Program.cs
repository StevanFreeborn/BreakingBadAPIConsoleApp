using System;
using Onspring.API.SDK;

namespace consoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            const string baseUrl = "";
            const string apiKey = "";
            var onspringClient = new OnspringClient(baseUrl, apiKey);
            Console.WriteLine("Hello World");
        }
    }
}
