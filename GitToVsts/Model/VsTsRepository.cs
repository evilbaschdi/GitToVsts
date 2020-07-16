using System.Runtime.Serialization;

namespace GitToVsts.Model
{
    /// <summary>
    ///     Visual Studio Team Services repository.
    /// </summary>
    [DataContract]
    public class VsTsRepository
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        ///     Name of the repository.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Project the repository belongs to.
        /// </summary>
        [DataMember]
        public VsTsProject Project { get; set; }

        /// <summary>
        ///     RemoteUrl for pushing / pulling.
        /// </summary>
        [DataMember]
        public string RemoteUrl { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Url { get; set; }
    }
}