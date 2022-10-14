using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class VsTsProcessTemplate
{
    /// <summary>
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Id { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public bool IsDefault { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Url { get; set; }
}