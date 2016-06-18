using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitRepository
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Full_Name { get; set; }

        [DataMember]
        public Owner Owner { get; set; }

        [DataMember]
        public bool @private { get; set; }

        [DataMember]
        public string Html_Url { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool Fork { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Forks_Url { get; set; }

        [DataMember]
        public string Keys_Url { get; set; }

        [DataMember]
        public string Collaborators_Url { get; set; }

        [DataMember]
        public string Teams_Url { get; set; }

        [DataMember]
        public string Hooks_Url { get; set; }

        [DataMember]
        public string Issue_Events_Url { get; set; }

        [DataMember]
        public string Events_Url { get; set; }

        [DataMember]
        public string Assignees_Url { get; set; }

        [DataMember]
        public string Branches_Url { get; set; }

        [DataMember]
        public string Tags_Url { get; set; }

        [DataMember]
        public string Blobs_Url { get; set; }

        [DataMember]
        public string Git_Tags_Url { get; set; }

        [DataMember]
        public string Git_Refs_Url { get; set; }

        [DataMember]
        public string Trees_Url { get; set; }

        [DataMember]
        public string Statuses_Url { get; set; }

        [DataMember]
        public string Languages_Url { get; set; }

        [DataMember]
        public string Stargazers_Url { get; set; }

        [DataMember]
        public string Contributors_Url { get; set; }

        [DataMember]
        public string Subscribers_Url { get; set; }

        [DataMember]
        public string Subscription_Url { get; set; }

        [DataMember]
        public string Commits_Url { get; set; }

        [DataMember]
        public string Git_Commits_Url { get; set; }

        [DataMember]
        public string Comments_Url { get; set; }

        [DataMember]
        public string Issue_Comment_Url { get; set; }

        [DataMember]
        public string Contents_Url { get; set; }

        [DataMember]
        public string Compare_Url { get; set; }

        [DataMember]
        public string Merges_Url { get; set; }

        [DataMember]
        public string Archive_Url { get; set; }

        [DataMember]
        public string Downloads_Url { get; set; }

        [DataMember]
        public string Issues_Url { get; set; }

        [DataMember]
        public string Pulls_Url { get; set; }

        [DataMember]
        public string Milestones_Url { get; set; }

        [DataMember]
        public string Notifications_Url { get; set; }

        [DataMember]
        public string Labels_Url { get; set; }

        [DataMember]
        public string Releases_Url { get; set; }

        [DataMember]
        public string Deployments_Url { get; set; }

        [DataMember]
        public string Created_At { get; set; }

        [DataMember]
        public string Updated_At { get; set; }

        [DataMember]
        public string Pushed_At { get; set; }

        [DataMember]
        public string Git_Url { get; set; }

        [DataMember]
        public string Ssh_Url { get; set; }

        [DataMember]
        public string Clone_Url { get; set; }

        [DataMember]
        public string Svn_Url { get; set; }

        [DataMember]
        public string Homepage { get; set; }

        [DataMember]
        public int Size { get; set; }

        [DataMember]
        public int Stargazers_Count { get; set; }

        [DataMember]
        public int Watchers_Count { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public bool Has_Issues { get; set; }

        [DataMember]
        public bool Has_Downloads { get; set; }

        [DataMember]
        public bool Has_Wiki { get; set; }

        [DataMember]
        public bool Has_Pages { get; set; }

        [DataMember]
        public int Forks_Count { get; set; }

        [DataMember]
        public object Mirror_Url { get; set; }

        [DataMember]
        public int Open_Issues_Count { get; set; }

        [DataMember]
        public int Forks { get; set; }

        [DataMember]
        public int Open_Issues { get; set; }

        [DataMember]
        public int Watchers { get; set; }

        [DataMember]
        public string Default_Branch { get; set; }

        [DataMember]
        public Permissions Permissions { get; set; }
    }
}