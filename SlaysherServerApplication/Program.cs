using SlaysherServer;

namespace SlaysherServerApplication
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
        }
    }
}