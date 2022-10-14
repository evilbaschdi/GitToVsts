using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class VsTsProjects
{
    /// <summary>
    /// </summary>
    [DataMember]
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public List<VsTsProject> Value { get; set; }
}