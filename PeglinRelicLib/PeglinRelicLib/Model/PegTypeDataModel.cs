using PeglinRelicLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Peg;

namespace PeglinRelicLib.Model
{
    class PegTypeDataModel : IModel<PegType>
    {
        public string GUID { get; set; }
    }
}
