namespace OSU_Helpers
{
    /// <summary>
    /// Aria connection strings for different environments at OSU
    /// </summary>
    public static class AriaConnection
    {
        /// <summary>
        /// Production connection string
        /// </summary>
        /// <returns>Production connection string</returns>
        public static string Prod()
        {
            return "Server=rad-ariadb-vp01;Initial Catalog=VARIAN;User Id=reports;Password=reports;TrustServerCertificate=True;Encrypt=True";
        }

        /// <summary>
        /// Dev_Physics connection string
        /// </summary>
        /// <returns>Dev_Physics connection string</returns>
        public static string Dev_Physics()
        {
            return "Server=rad-physic-pd01;Initial Catalog=VARIAN;User Id=reports;Password=reports;TrustServerCertificate=True;Encrypt=True";
        }

        /// <summary>
        /// Dev_Script connection string
        /// </summary>
        /// <returns>Dev_Script connection string</returns>
        public static string Dev_Script()
        {
            return "Server=rad-script-pd01;Initial Catalog=VARIAN;User Id=reports;Password=reports;TrustServerCertificate=True;Encrypt=True";
        }

        /// <summary>
        /// Aura datawarehouse connection string
        /// </summary>
        /// <returns>Aura datawarehouse connection string</returns>
        public static string Aura()
        {
            return "Server=rad-ardwh-vp01;Initial Catalog=VARIAN;User Id=reports;Password=reports;TrustServerCertificate=True;Encrypt=True";
        }
    }
}
