using System;
using System.IO;

namespace wmspanel_plugin
{
    class IPListLoader
    {
        static public IPList loadIpList(String fileName){
            IPList list = new IPList();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] ip_mask = line.Split('/');
                        if (ip_mask.Length == 2)
                        {
                            list.Add(ip_mask[0], Convert.ToInt32(ip_mask[1]));
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return list;
        }
    }
}
