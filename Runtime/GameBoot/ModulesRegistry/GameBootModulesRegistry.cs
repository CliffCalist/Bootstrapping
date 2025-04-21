using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    /// <summary>
    /// Registry for storing the list of bootstrap module type names.
    /// This ScriptableObject ensures type data is preloaded in builds and reduces runtime reflection.
    /// </summary>
    public class GameBootModulesRegistry : ScriptableObject
    {
        public bool BootstrapingIsEnabled = true;
        public List<string> ModuleTypeNames = new();


        public const string FILE_NAME = "GameBootstrapModulesRegistry";
    }
}