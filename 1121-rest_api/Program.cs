using System.Net;

namespace _1121_rest_api
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = HttpListener;
            app.Prefixes.Add("http://localhost:5000/");
            app.Start();

            Console.WriteLine("Listening on http://localhost:5000/ ...");



        }
    }
}
