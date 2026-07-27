using System.Runtime.Serialization;

namespace GitToVsts.Core.Model;

/// <summary>
/// </summary>
[DataContract]
public class DevOpsProcessTemplates
{
    /// <summary>
    /// </summary>
    [DataMember]
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public List<DevOpsProcessTemplate> Value { get; set; }
}