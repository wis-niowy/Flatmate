using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Helpers
{
    public enum Frequency
    {
        EveryDay,
        EveryWeek,
        Every2Weeks,
        Every3Weeks,
        EveryMonth,
        Every2Months,
        Every3Months,
        Every6Months,
        EveryYear
    }

    public enum ExpenseCategory
    {
        Shopping,
        Bill,
        Other
    }

    public enum Unit
    {
        Miligram,
        Gram,
        Decagram,
        Kilogram,
        Tone,
        Mililiter,
        Liter,
        Piece,
        Other
    }
}
