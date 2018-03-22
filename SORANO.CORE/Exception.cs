using System;

namespace SORANO.CORE
{
    public class Exception : Entity
    {
        public string Message { get; set; }

        public string InnerException { get; set; }

        public string StackTrace { get; set; }

        public DateTime Timestamp { get; set; }
    }
}