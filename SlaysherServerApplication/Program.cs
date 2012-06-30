using SlaysherServer;

namespace SlaysherServerApplication
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Server server = new Server();
            server.Run();
        }
    }
}