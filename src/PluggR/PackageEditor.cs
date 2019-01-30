using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR
{
    internal class PackageEditor : Editor
    {
        public override Task ApplyAsync(EditorContext context, Operation operation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (operation is AddOperation addOperation)
            {
                var projectContext = context.GetData<IProjectContext>();
                InstallPackage(projectContext.ProjectFullPath, addOperation);
            }

            return Task.CompletedTask;
        }

        private void InstallPackage(string projectPath, AddOperation addOperation)
        {
            var argumentString = $"add {projectPath} package {addOperation.PackageName}";
            if (!string.IsNullOrEmpty(addOperation.PackageVersion))
            {
                argumentString += $" --version { addOperation.PackageVersion}";
            }

            var proc = Process.Start("dotnet", argumentString);
            proc.WaitForExit();
        }
    }
}
