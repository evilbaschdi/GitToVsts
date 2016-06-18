namespace GitToVsts.Internal.Models
{
    public class GitRepositoryObservableCollectionItem
    {
        /// <summary>
        ///     Displayname of the GitRepository.
        /// </summary>
        public string Displayname { get; set; }

        /// <summary>
        ///     True if the GitRepository should be migrated to VsTs.
        /// </summary>
        public bool MigrateToVsTs { get; set; }

        /// <summary>
        ///     GitRepository of current Item.
        /// </summary>
        public GitRepository Repository { get; set; }
    }
}