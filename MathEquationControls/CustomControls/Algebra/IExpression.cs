using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquationControls.CustomControls.Algebra
{
    public static class Expression
    {
        public static Empty Empty = new Empty();
    }


    public interface IExpression
    {
        public string Text { get; set; }
    }
}
