using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class VsTsProjects
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<VsTsProject> Value { get; set; }
    }
}