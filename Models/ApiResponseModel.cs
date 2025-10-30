using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class ApiResponseModel<T>
    {
        public T Message { get; set; }
    }
}
