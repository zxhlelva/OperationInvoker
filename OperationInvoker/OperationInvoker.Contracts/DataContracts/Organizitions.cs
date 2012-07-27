using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OperationInvoker.CustomServiceBehaviors;

namespace OperationInvoker.Contracts.DataContracts
{
    [Serializable]
    [DataContract]
    public class Organizitions
    {
        [DataMember]
        public string ResponseLink { get; set; }

        [DataMember]
        public List<string> Organizition { get; set; }
    }
}
