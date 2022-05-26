using PeglinRelicLib.Register;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Peg;

namespace PeglinRelicLib.Utility
{
    public static class RegisterExtensionMethods
    {
        public static bool Is(this RelicEffect effect, string GUID)
        {
            if(RelicRegister.TryGetCustomRelicEffect(GUID, out RelicEffect effect2))
            {
                return effect == effect2;
            }
            return false;
        }
        public static bool Is(this PegType effect, string GUID)
        {
            if (PegTypeRegister.TryGetCustomPegType(GUID, out PegType effect2))
            {
                return effect == effect2;
            }
            return false;
        }
    }
}
