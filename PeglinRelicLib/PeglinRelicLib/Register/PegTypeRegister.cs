using PeglinRelicLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Peg;

namespace PeglinRelicLib.Register
{
    public class PegTypeRegister : Register<PegTypeRegister, PegType>
    {
        public override string SaveID => "io.github.crazyjackel.pegTypeRegister";

        protected override PegType CalculateEnum(string GUID)
        {
            throw new NotImplementedException();
        }

        protected override bool LoadModel(IModel<PegType> model, out object args)
        {
            throw new NotImplementedException();
        }

        protected override void UnloadEnum(PegType @enum)
        {
            throw new NotImplementedException();
        }
    }
}
