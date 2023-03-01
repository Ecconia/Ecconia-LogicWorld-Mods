// File: ModHelper.cs
// Project: AssemblyLoader
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using LogicAPI.Modding;
using LogicAPI;
using LogicWorld.SharedCode.Modding;
using SUCC;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AssemblyLoader.Shared;

public class ModHelper
{
    public static IEnumerable<ModFile> GetDependentModFiles(IModFiles files, string modID)
    {
        foreach (var dir in Directory.EnumerateDirectories(Path.Combine(files.Path, ".."), "*"))
        {
            var manifest = new DataFile(Path.Combine(dir, "manifest.succ")).GetAsObject<ModManifest>();
            if (!manifest.Dependencies.Contains(modID)) continue;

            var modFiles = new FolderModFiles(dir);

            foreach (var file in modFiles.EnumerateFiles("*.dll"))
            {
                yield return file;
            }
        }
    }
}