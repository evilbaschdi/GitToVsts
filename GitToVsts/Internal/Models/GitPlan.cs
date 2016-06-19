using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitPlan
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Space { get; set; }

        [DataMember]
        public int Collaborators { get; set; }

        [DataMember]
        public int Private_Repos { get; set; }
    }
}