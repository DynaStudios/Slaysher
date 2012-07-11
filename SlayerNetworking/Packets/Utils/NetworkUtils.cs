using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
                        if (string.IsNullOrEmpty(addr))
                            outValue = iphe.AddressList[iphe.AddressList.Length - 2];
                        else
                            outValue = iphe.AddressList[iphe.AddressList.Length - 1];
                        return true;
                    }
                }
                catch
                {
                }
            }

            outValue = IPAddress.None;
            return false;
        }
    }
}