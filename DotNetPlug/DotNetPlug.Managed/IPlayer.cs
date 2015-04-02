using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Player info Interface
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Gets the player Id.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        int Id { get; }
    }
}
