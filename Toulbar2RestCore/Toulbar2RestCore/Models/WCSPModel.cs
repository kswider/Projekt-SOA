using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toulbar2RestCore.Models.InternalClasses;

namespace Toulbar2RestCore.Models
{
    public class WCSPModel
    {
        public int UpperBound { get; set; }
        public List<Variable> Variables { get; set; }
        public List<Function> Functions { get; set; }
    }

}
