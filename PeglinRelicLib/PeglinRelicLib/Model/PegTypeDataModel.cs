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
        public Dictionary<Type, object> Actions { get; set; } = new Dictionary<Type, object>();

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
            Actions.Add(typeof(T), OnConversion);
            return this;
        }
    }
}
