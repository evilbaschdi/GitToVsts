using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace GitToVsts.Model;

/// <summary>
/// </summary>
[DataContract]
public class GitPlan
{
    /// <summary>
    /// </summary>
    [DataMember]
    public int Collaborators { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    // ReSharper disable once InconsistentNaming
    public int Private_Repos { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Space { get; set; }
}