using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toulbar2RestCore.Models.InternalClasses;

namespace Toulbar2RestCore.Models
{
    public class ResponseModel
    {
        public List<Variable> Variables { get; set; } = new List<Variable>();
        public double AccomplishementPercentage { get; set; }
        public double Time { get; set; }
        public int Memory { get; set; }
        public string RawOutput { get; set; }

    }
}
