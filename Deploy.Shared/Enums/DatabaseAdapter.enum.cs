using System;
using System.Collections.Generic;
using System.Text;

namespace Deploy.Shared.Enums
{
    /// <summary>
    /// Which database adapter to use for SQLCommands
    /// </summary>
    public enum DatabaseAdapter
    {
        None = 0,
        MSSQL = 1,
        MySQL = 2
    }
}
