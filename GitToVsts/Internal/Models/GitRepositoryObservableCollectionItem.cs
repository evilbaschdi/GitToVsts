using System.Runtime.Serialization;

namespace GitToVsts.Internal.Models
{
    /// <summary>
    ///     ObservableColletion to contain GitRepositories.
    /// </summary>
    [DataContract]
    public class GitRepositoryObservableCollectionItem
    {
        /// <summary>
        ///     Displayname of the GitRepository.
        /// </summary>
        [DataMember]
        public string Displayname { get; set; }

        /// <summary>
        ///     True if the GitRepository should be migrated to VsTs.
        /// </summary>
        [DataMember]
        public bool MigrateToVsTs { get; set; }

        /// <summary>
        ///     True if the GitRepository was successfully migrated to VsTs.
        /// </summary>
        [DataMember]
        public bool MigrationSuccessful { get; set; }

        /// <summary>
        ///     GitRepository of current Item.
        /// </summary>
        [DataMember]
        public GitRepository Repository { get; set; }
    }
}