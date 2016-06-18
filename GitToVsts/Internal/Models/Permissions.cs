using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class Permissions
    {
        [DataMember]
        public bool Admin { get; set; }

        [DataMember]
        public bool Push { get; set; }

        [DataMember]
        public bool Pull { get; set; }
    }
}