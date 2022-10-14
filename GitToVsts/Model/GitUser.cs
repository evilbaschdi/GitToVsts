using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace GitToVsts.Model;

/// <summary>
///     Defines a Github user.
/// </summary>
[DataContract]
public class GitUser
{
    /// <summary>
    /// </summary>
    [DataMember]
    public string Avatar_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public object Bio { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Blog { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Collaborators { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public object Company { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Created_At { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Disk_Usage { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public object Email { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Events_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Followers { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Followers_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Following { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Following_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Gists_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    // ReSharper disable once IdentifierTypo
    public string Gravatar_Id { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    // ReSharper disable once IdentifierTypo
    public object Hireable { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Html_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public object Location { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Login { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Organizations_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Owned_Private_Repos { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public GitPlan Plan { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Private_Gists { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Public_Gists { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Public_Repos { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Received_Events_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Repos_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public bool Site_Admin { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Starred_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Subscriptions_Url { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public int Total_Private_Repos { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Updated_At { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Url { get; set; }
}