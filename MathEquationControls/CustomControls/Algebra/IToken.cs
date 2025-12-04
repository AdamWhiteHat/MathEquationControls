using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquationControls.CustomControls.Algebra
{
    public interface IToken<T> : IExpression
    {
        public T Value { get; set; }
    }
}
