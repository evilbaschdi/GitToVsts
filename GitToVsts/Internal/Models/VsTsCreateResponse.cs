using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class VsTsCreateResponse
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Url { get; set; }
    }
}