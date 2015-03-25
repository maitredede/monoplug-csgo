using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public interface IEngine
    {
        Task<string> ExecuteCommand(string command);
        Task Log(string log);
    }
}
