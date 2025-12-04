using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquationControls.CustomControls.Algebra
{
    public interface IBinaryExpression : IExpression
    {
        public IExpression LHS { get; set; }
        public IExpression RHS { get; set; }
    }
}
