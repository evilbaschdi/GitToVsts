using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    /// <summary>
    ///     Visual Studio Team Services project.
    /// </summary>
    [DataContract]
    public class VsTsProject
    {
        /// <summary>
        ///     Id of the project.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public int Revision { get; set; }
    }
}