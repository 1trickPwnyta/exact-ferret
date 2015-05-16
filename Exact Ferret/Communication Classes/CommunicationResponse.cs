using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Exact_Ferret
{
    [DataContract]
    internal class CommunicationResponse
    {
        [DataMember]
        internal bool success;

        [DataMember]
        internal string reason;

        [DataMember]
        internal int countdown;
    }
}
