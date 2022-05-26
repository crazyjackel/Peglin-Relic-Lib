using Battle;
using PeglinRelicLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Peg;

namespace PeglinRelicLib.Model
{
    public class PegTypeDataModel : IModel<PegType>
    {
        public string GUID { get; set; }

        public HashSet<Type> SupportTypes { get; set; } = new HashSet<Type>();
        public Dictionary<Type, object> PegConversionAction { get; set; } = new Dictionary<Type, object>();
        public Func<PegManager, int> GetPegCount { get; set; } = null;
        public PegTypeDataModel(string GUID)
        {
            this.GUID = GUID;
        }

        public PegTypeDataModel AddSupport<T>() where T : Peg
        {
            SupportTypes.Add(typeof(T));
            return this;
        }
        public PegTypeDataModel AddSupport<T>(Action<T> OnConversion) where T : Peg
        {
            SupportTypes.Add(typeof(T));
            PegConversionAction.Add(typeof(T), OnConversion);
            return this;
        }

        public PegTypeDataModel AddBoardPegCount(Func<PegManager,int> GetPegCount)
        {
            this.GetPegCount = GetPegCount;
            return this;
        }
    }
}
