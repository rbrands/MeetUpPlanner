using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUpPlanner.Server.Repositories
{
    /// <summary>
    /// Interface to use the functions provided by MeetUpFunctions from Azure Function App
    /// </summary>
    public interface IMeetUpFunctions
    {
        Task<string> GetVersion();
    }
}
