using Flatmate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Models.EntityModels
{
    public class SingleOrderElement
    {
        public int Id { get; set; }
        public int SCOId { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public Unit Unit { get; set; }
        public SingleComplexOrder SingleComplexOrder { get; set; }
    }
}
