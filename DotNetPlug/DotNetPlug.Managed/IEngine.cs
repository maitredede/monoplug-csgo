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

        Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback);
        void RaiseCommand(int id, int argc, string[] argv);
        Task UnregisterCommand(int id);
    }
}
