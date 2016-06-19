using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class VsTsProcessTemplates
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<VsTsProcessTemplate> Value { get; set; }
    }
}