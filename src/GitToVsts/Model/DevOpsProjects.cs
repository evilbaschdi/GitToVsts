using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class DevOpsProjects
{
    /// <summary>
    /// </summary>
    [DataMember]
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public List<DevOpsProject> Value { get; set; }
}