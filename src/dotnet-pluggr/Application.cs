using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Pluggr
{
    internal class Application : CommandLineApplication
    {
        public Application()
        {
            Commands.Add(new AnalyzeCommand());
        }
    }
}
