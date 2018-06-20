using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace PluggR.Analysis
{
    public class StartupModel
    {
        public StartupModel(
            SyntaxTree syntaxTree,
            List<ServiceRegistrationModel> services, 
            List<MiddlewareRegistrationModel> middleware)
        {
            SyntaxTree = syntaxTree;
            Services = services;
            Middleware = middleware;
        }

        public List<MiddlewareRegistrationModel> Middleware { get; }
        public List<ServiceRegistrationModel> Services { get; }
        public SyntaxTree SyntaxTree { get; }
    }
}