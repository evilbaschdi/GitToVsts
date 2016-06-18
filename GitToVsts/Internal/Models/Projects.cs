using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class Projects
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<Project> Value { get; set; }
    }
}