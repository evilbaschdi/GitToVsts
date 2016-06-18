using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class ProcessTemplates
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<ProcessTemplate> Value { get; set; }
    }
}