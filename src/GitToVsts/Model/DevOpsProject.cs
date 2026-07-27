using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
///     Azure DevOps project.
/// </summary>
[DataContract]
public class DevOpsProject
{
    /// <summary>
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    ///     Id of the project.
    /// </summary>
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Revision { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string State { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Url { get; set; }
}