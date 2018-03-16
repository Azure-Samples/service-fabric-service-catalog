using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.Commands
{
    public interface ISchemaChecker
    {
        bool IsValid(string payload);
    }
}
