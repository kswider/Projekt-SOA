using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toulbar2RestCore.Models.InternalClasses
{
    public class Variable
    {
        public string Name { get; set; }
        public int MaxVal { get; set; }
        public bool Extreme { get; set; }
        public double Value { get; set; }
    }
}
