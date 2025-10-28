using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Interface
{
    public interface IUserConfigRepository
    {
        void Save(string data);

        string? Load();

        void Delete();
    }
}
