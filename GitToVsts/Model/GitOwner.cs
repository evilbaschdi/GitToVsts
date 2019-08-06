using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Model
{
    /// <summary>
    ///     Owner of a git repository.
    /// </summary>
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitOwner
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string Avatar_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Events_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Followers_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Following_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Gists_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        // ReSharper disable once IdentifierTypo
        public string Gravatar_Id { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Html_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Login { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Organizations_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Received_Events_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Repos_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Site_Admin { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Starred_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Subscriptions_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Url { get; set; }
    }
}