using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates project through visualstudio.com api.
    /// </summary>
    public interface ICreateProject : IValue<VsTsCreateResponse>
    {
    }
}