using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public class BootSettings : ScriptableObject
    {
        public bool BootstrapingIsEnabled = true;
        public List<string> ModuleTypeNames = new();


        public const string FILE_NAME = "BootSettings";
    }
}