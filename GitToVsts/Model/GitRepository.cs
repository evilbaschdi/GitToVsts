using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace GitToVsts.Model
{
    /// <summary>
    /// </summary>
    [DataContract]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GitRepository
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Full_Name { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public GitOwner Owner { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool @private { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Html_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Fork { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Forks_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Keys_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Collaborators_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Teams_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Hooks_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Issue_Events_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Events_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Assignees_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Branches_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Tags_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Blobs_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Git_Tags_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Git_Refs_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Trees_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Statuses_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Languages_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Stargazers_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Contributors_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Subscribers_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Subscription_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Commits_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Git_Commits_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Comments_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Issue_Comment_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Contents_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Compare_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Merges_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Archive_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Downloads_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Issues_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Pulls_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Milestones_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Notifications_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Labels_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Releases_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Deployments_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Created_At { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Updated_At { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Pushed_At { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Git_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Ssh_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Clone_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Svn_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Homepage { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Size { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Stargazers_Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Watchers_Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Language { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Has_Issues { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Has_Downloads { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Has_Wiki { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool Has_Pages { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Forks_Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public object Mirror_Url { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Open_Issues_Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Forks { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Open_Issues { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public int Watchers { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Default_Branch { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public GitPermissions Permissions { get; set; }
    }
}