using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class VsTsRepositories
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public List<VsTsRepository> Value { get; set; }
    }
}