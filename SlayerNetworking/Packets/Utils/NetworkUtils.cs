using System;
using System.Net;

namespace SlaysherNetworking.Packets.Utils
{
    public class NetworkUtils
    {
        public static bool Resolve(string addr, out IPAddress outValue)
        {
            try
            {
                outValue = IPAddress.Parse(addr);
                return true;
            }
            catch
            {
                try
                {
                    IPHostEntry iphe = Dns.GetHostEntry(addr);

                    if (iphe.AddressList.Length > 0)
                    {
                        outValue = string.IsNullOrEmpty(addr) ? iphe.AddressList[iphe.AddressList.Length - 2] : iphe.AddressList[iphe.AddressList.Length - 1];
                        return true;
                    }
                }
                catch (Exception)
                {
                }
            }

            outValue = IPAddress.None;
            return false;
        }
    }
}