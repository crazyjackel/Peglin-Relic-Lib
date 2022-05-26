using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Interfaces
{
    public interface IRegister<T> where T : Enum
    {
        string SaveID { get; }
        List<T> Values { get; }
        bool TryGetRegisteredValue(string GUID, out T @enum);
        bool RegisterValue(IModel<T> model, out T @enum);
        void ReleaseValue(string GUID);
        void Save();
        void Load();
        void Reset();
    }
}
