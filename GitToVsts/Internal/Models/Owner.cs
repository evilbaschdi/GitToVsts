using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Owner
    {
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Avatar_Url { get; set; }

        [DataMember]
        public string Gravatar_Id { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Html_Url { get; set; }

        [DataMember]
        public string Followers_Url { get; set; }

        [DataMember]
        public string Following_Url { get; set; }

        [DataMember]
        public string Gists_Url { get; set; }

        [DataMember]
        public string Starred_Url { get; set; }

        [DataMember]
        public string Subscriptions_Url { get; set; }

        [DataMember]
        public string Organizations_Url { get; set; }

        [DataMember]
        public string Repos_Url { get; set; }

        [DataMember]
        public string Events_Url { get; set; }

        [DataMember]
        public string Received_Events_Url { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public bool Site_Admin { get; set; }
    }
}