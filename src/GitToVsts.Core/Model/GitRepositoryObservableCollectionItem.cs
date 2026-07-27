using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
///     ObservableCollection to contain GitRepositories.
/// </summary>
[DataContract]
public class GitRepositoryObservableCollectionItem
{
    /// <summary>
    ///     DisplayName of the GitRepository.
    /// </summary>
    [DataMember]
    public string DisplayName { get; set; }

    /// <summary>
    ///     True if the GitRepository should be migrated to DevOps.
    /// </summary>
    public bool MigrateToDevOps { get; set; }

    /// <summary>
    ///     True if the GitRepository was successfully migrated to DevOps.
    /// </summary>
    public bool MigrationSuccessful { get; set; }

    /// <summary>
    ///     GitRepository of current Item.
    /// </summary>
    [DataMember]
    public GitRepository Repository { get; set; }
}