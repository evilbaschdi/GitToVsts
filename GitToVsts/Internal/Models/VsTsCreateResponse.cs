using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    public class VsTsCreateResponse
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}