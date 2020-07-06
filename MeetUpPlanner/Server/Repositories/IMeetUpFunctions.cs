using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Repositories
{
    /// <summary>
    /// Interface to use the functions provided by MeetUpFunctions from Azure Function App
    /// </summary>
    public interface IMeetUpFunctions
    {
        /// <summary>
        /// Gets the version of the Azure functions module
        /// </summary>
        /// <returns></returns>
        Task<string> GetVersion();
        /// <summary>
        /// Gets settings to be used by the frontend client.
        /// </summary>
        /// <returns></returns>
        Task<ClientSettings> GetClientSettings();
    }
}
