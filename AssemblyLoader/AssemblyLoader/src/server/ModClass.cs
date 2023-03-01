using System;
using System.Linq;
using AssemblyLoader.Shared;
using LogicAPI.Server;

namespace AssemblyLoader.server
{
    public class ModClass : ServerMod
    {
        protected override void Initialize()
        {
            Logger.Info("AssemblyLoader Initialize...");

            foreach (var file in ModHelper.GetDependentModFiles(Files, Manifest.ID)
                         .Where(e => e.Path.Contains("server") || e.Path.Contains("shared")))
            {
                try
                {
                    DLLLoader.loadAssemblyFromModFile(file);
                    Logger.Info($"Load Assembly {file.FileName}");
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
        }
    }
}
