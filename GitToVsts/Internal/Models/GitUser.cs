using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitUser
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

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Company { get; set; }

        [DataMember]
        public string Blog { get; set; }

        [DataMember]
        public object Location { get; set; }

        [DataMember]
        public object Email { get; set; }

        [DataMember]
        public object Hireable { get; set; }

        [DataMember]
        public object Bio { get; set; }

        [DataMember]
        public int Public_Repos { get; set; }

        [DataMember]
        public int Public_Gists { get; set; }

        [DataMember]
        public int Followers { get; set; }

        [DataMember]
        public int Following { get; set; }

        [DataMember]
        public string Created_At { get; set; }

        [DataMember]
        public string Updated_At { get; set; }

        [DataMember]
        public int Private_Gists { get; set; }

        [DataMember]
        public int Total_Private_Repos { get; set; }

        [DataMember]
        public int Owned_Private_Repos { get; set; }

        [DataMember]
        public int Disk_Usage { get; set; }

        [DataMember]
        public int Collaborators { get; set; }

        [DataMember]
        public Plan Plan { get; set; }
    }
}