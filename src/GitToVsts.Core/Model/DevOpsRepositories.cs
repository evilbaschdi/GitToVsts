using System.Runtime.Serialization;

namespace GitToVsts.Core.Model;

/// <summary>
/// </summary>
[DataContract]
public class DevOpsRepositories
{
    /// <summary>
    /// </summary>
    [DataMember]
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public List<DevOpsRepository> Value { get; set; }
}