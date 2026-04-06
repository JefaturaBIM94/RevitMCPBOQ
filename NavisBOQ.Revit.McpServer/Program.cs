using System;
using System.Text;

namespace NavisBOQ.Revit.McpServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            var server = new NavisBOQ.Revit.McpServer.Mcp.McpServer();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string responseJson = server.Handle(line);
                if (!string.IsNullOrWhiteSpace(responseJson))
                {
                    Console.WriteLine(responseJson);
                    Console.Out.Flush();
                }
            }
        }
    }
}