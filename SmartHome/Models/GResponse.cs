using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GResponse<T>
    {
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
