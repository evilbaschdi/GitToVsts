using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class VsTsProcessTemplates
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public List<VsTsProcessTemplate> Value { get; set; }
    }
}