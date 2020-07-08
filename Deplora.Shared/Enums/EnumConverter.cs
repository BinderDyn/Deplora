using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.Shared.Enums
{
    /// <summary>
    /// Enum utility for converting enums to strings and vice versa
    /// </summary>
    public static class EnumConverter
    {
        /// <summary>
        /// Gets the string for the DatabaseAdapter enum
        /// </summary>
        /// <param name="databaseAdapter"></param>
        /// <returns></returns>
        public static string ConvertDatabaseAdapterToString(DatabaseAdapter databaseAdapter)
        {
            return databaseAdapter switch
            {
                DatabaseAdapter.MSSQL => "MSSQL",
                DatabaseAdapter.MySQL => "MySQL",
                DatabaseAdapter.None => "None",
                _ => throw new NotImplementedException(databaseAdapter.ToString())
            };
        }

        public static Dictionary<string, T> GetSelectItemsInferredFromEnum<T>() where T : Enum
        {
            var dict = new Dictionary<string, T>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                dict.Add(Enum.GetName(typeof(T), e), (T)e);
            }
            return dict;
        }
    }
}
