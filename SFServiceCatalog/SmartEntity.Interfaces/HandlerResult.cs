using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFServiceCatalog.SmartEntity.Interfaces
{
    public class HandlerResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public string UpdatedEntity { get; set; }
        public string UpdatedState { get; set; }
        public System.Net.HttpStatusCode HTTPStatus { get; set; }
    }
}
