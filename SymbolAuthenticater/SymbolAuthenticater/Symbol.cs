using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;

namespace SymbolAuthenticater
{
    [System.Serializable]
    internal class Root
    {
        public Account account;
    }
    [System.Serializable]
    internal class Account
    {
        public Mosaic[] mosaics;
    }
    [System.Serializable]
    internal class Mosaic
    {
        public string id;
        public int amount;
    }

    internal class Seat
    {
        public static Dictionary<string, string> Seat_ticket = new Dictionary<string, string>
        {
            {"Id1","SS_Seat"},
            {"Id2","S_Seat"},
            {"Id3" ,"A_Seat"},
            {"Id4" ,"B_Seat"},
            {"Id5" ,"Standing_Seat"},
        };
    } 

    internal class Symbol
    {
        public static string GetDataFromAPI(string node, string address)
        {
            var url = node + "/accounts/"+ address;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "application/json";
            req.Method = "GET";
            try 
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                using (res)
                {
                    using (var resStream = res.GetResponseStream())
                    {
                        try
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Root));
                            var root = (Root)serializer.ReadObject(resStream);
                            foreach (Mosaic Mo in root.account.mosaics)
                            {
                                if (Seat.Seat_ticket.TryGetValue(Mo.id, out string seat))
                                {
                                    return seat;
                                }
                            }
                            return null;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            return null;
                        }
                    }

                }
            }
            catch(Exception e)
            {
                return "E1";
            }
            

        }
    }

}
