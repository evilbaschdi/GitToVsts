using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class ProcessTemplate
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}