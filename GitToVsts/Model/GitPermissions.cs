using System.Runtime.Serialization;

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class GitPermissions
{
    /// <summary>
    /// </summary>
    [DataMember]
    public bool Admin { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public bool Pull { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public bool Push { get; set; }
}