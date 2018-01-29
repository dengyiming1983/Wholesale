using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceModel.MoleQPos
{
    public class MessageContent
    {
        public string table_name { get; set; }
        public string action_type { get; set; }
        public string update_column { get; set; }
        public string pk { get; set; }
        public object content { get; set; }
    }
}
