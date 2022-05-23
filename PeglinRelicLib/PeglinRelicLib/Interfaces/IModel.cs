using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeglinRelicLib.Interfaces
{
    public interface IModel<T> where T : Enum
    {
        string GUID { get; }
    }
}
