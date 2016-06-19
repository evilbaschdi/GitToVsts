using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class GitPermissions
    {
        [DataMember]
        public bool Admin { get; set; }

        [DataMember]
        public bool Push { get; set; }

        [DataMember]
        public bool Pull { get; set; }
    }
}