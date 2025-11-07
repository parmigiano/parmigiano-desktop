using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parmigiano.Interface
{
    public interface IUserConfigRepository
    {
        string? GetString(string key);

        void Set(string key, string value);

        void DeleteKey(string key);
    }
}
