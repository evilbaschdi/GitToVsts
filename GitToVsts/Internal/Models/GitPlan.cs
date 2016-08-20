using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitPlan
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Space { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Collaborators { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Private_Repos { get; set; }
    }
}