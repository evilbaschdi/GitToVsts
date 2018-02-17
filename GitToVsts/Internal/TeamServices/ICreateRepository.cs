using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates repository through visualstudio.com api.
    /// </summary>
    public interface ICreateRepository : IValue<VsTsRepository>
    {
    }
}