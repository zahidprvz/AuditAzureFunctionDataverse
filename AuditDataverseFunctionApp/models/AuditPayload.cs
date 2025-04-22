using System;
using System.Collections.Generic;

namespace AuditDataverseFunctionApp.Models
{
    public class AuditPayload
    {
        public List<AuditEntry> value { get; set; }
    }

    public class AuditEntry
    {
        public string auditid { get; set; }
        public DateTime createdon { get; set; }
        public string objecttypecode { get; set; }
        public string _objectid_value { get; set; }
        public string _userid_value { get; set; }
        public string changedata { get; set; }
    }
}
