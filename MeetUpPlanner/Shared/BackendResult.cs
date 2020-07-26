using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Object used as return value of functions in the backend.
    /// </summary>
    public class BackendResult
    {
        /// <summary>
        /// true if operation was successfull
        /// </summary>
        public bool Success { get; set; }
        public string Message { get; set; }

        public BackendResult()
        {

        }
        public BackendResult(bool status)
        {
            Success = status;
        }
        public BackendResult(bool status, string message)
        {
            Success = status;
            Message = message;
        }
    }
}
