using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class DevOpsCreateResponse
{
    /// <summary>
    /// </summary>
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Status { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Url { get; set; }
}
