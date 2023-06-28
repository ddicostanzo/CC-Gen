using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// Citrix Server Selection for use with DICOM transactions
    /// </summary>
    public class CitrixServerSelection
    {
        /// <summary>
        /// IP address of the server
        /// </summary>
        public string ip_address;
        /// <summary>
        /// AE title of the server
        /// </summary>
        public string ae_title;
        /// <summary>
        /// Port to perform transaction on
        /// </summary>
        public int port;

        /// <summary>
        /// IP address of server that the user is connected to
        /// </summary>
        internal string user_ip = IpHelper.GetLocalIPAddress().ToString();
        /// <summary>
        /// Dictionary of servers suffix with assigned IP addresses
        /// </summary>
        internal Dictionary<string, string> citrix_servers;

        /// <summary>
        /// Construcor that sets IP addresses of the Citrix Servers
        /// </summary>
        public CitrixServerSelection()
        {
            citrix_servers = new Dictionary<string, string>();

            citrix_servers.Add("01", "10.95.17.30");
            citrix_servers.Add("02", "10.95.17.32");
            citrix_servers.Add("03", "10.95.17.15");
            citrix_servers.Add("04", "10.95.17.14");
            citrix_servers.Add("05", "10.95.17.29");
            citrix_servers.Add("06", "10.95.17.16");
            citrix_servers.Add("07", "10.95.17.33");
            citrix_servers.Add("08", "10.95.17.34");
            citrix_servers.Add("09", "10.95.17.11");
            citrix_servers.Add("10", "10.95.17.31");
            citrix_servers.Add("11", "10.95.17.5");
            citrix_servers.Add("12", "10.95.17.37");
            citrix_servers.Add("13", "10.95.17.40");
            citrix_servers.Add("14", "10.95.17.38");
            citrix_servers.Add("15", "10.95.17.39");
            citrix_servers.Add("16", "10.95.17.8");
            citrix_servers.Add("17", "10.95.17.41");
            citrix_servers.Add("18", "10.95.17.36");
            citrix_servers.Add("19", "10.95.17.43");
            citrix_servers.Add("00", "10.65.160.105");
            //citrix_servers.Add("20", "10.95.17.32");

        }

    }

    /// <summary>
    /// Inherited class from Citrix Server Selection that assigns appropriate ports and AE title based upon the IP address for the SCU
    /// </summary>
    public class DCM_SCU : CitrixServerSelection
    {
        /// <summary>
        /// Inheritied class with specific port and AE title for SCU
        /// </summary>
        public void server_based_on_ip()
        {
            string svr_name = citrix_servers.First(a => a.Value == user_ip).Key;

            port = 50401;
            ae_title = "DCM" + svr_name + "_C";
            ip_address = IpHelper.GetLocalIPAddress().ToString();
        }
    }

    /// <summary>
    /// Inherited class from Citrix Server Selection that assigns appropriate ports and AE title based upon the IP address for the SCP
    /// </summary>
    public class DCM_SCP : CitrixServerSelection
    {
        /// <summary>
        /// Inheritied class with specific port and AE title for SCP
        /// </summary>
        public void server_based_on_ip()
        {
            string svr_name = citrix_servers.First(a => a.Value == user_ip).Key;

            port = 50400;
            ae_title = "DCM" + svr_name;
            ip_address = IpHelper.GetLocalIPAddress().ToString();
        }
    }


}
