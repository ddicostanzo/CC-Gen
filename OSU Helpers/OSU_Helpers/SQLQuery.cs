using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OSU_Helpers
{
    /// <summary>
    /// Queries the input DB with the given SQL Select statement, and will add parameters if provided.
    /// </summary>
    public static class SQLQuery
    {
        /// <summary>
        /// Query Varian database for specific data.
        /// </summary>
        /// <param name="Query">String input of the Select statement to run against the Varian database.</param>
        /// <param name="ConnectionString">The connection string to use to run the query. Check AriaConnection helper for strings.</param>
        /// <param name="QueryParameters">Dictionary of parameters to use in the query where the Keys should be @Parameter and the values should be Values of the parameters.</param>
        /// <returns>A DataTable is returned with the rows corresponding to the results of the query.</returns>
        public static DataTable QueryResults(string Query, string ConnectionString, Dictionary<string, object> QueryParameters)
        {
            string queryString = Query;

            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                if (QueryParameters != null)
                {
                    command.Parameters.AddRange(DictionaryToSqlParameterArray(QueryParameters));
                }
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Converts a Dictionary to an array of SqlParameters
        /// </summary>
        /// <param name="parameters">Dictionary of SQL parameters</param>
        /// <returns>Array of SQLParameters</returns>
        private static SqlParameter[] DictionaryToSqlParameterArray(Dictionary<string, object> parameters)
        {
            var sqlParameterCollection = new List<SqlParameter>();
            foreach (var parameter in parameters)
            {
                sqlParameterCollection.Add(new SqlParameter(parameter.Key, parameter.Value));
            }
            return sqlParameterCollection.ToArray();
        }
    }
}
