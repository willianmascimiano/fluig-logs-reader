using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluigLogsReader
{
    internal class LogFileModel
    {

        public DateTime dateTime { get; set;  } 
        public String logType { get; set; }

        public String logClassName { get; set; }

        public String thread {  get; set; }

        public String wildflyError { get; set; }

        public String message { get; set;  }


    }
}
