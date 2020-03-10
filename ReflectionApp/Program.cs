using System;

namespace ReflectionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ReflectionWorker worker = new ReflectionWorker();
            worker.Start();

            Console.ReadLine();
        }
    }
}
