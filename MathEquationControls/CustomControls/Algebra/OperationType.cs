using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquationControls.CustomControls.Algebra
{
    public enum OperationType
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public static class OperationTypeHelper
    {
        public static Dictionary<char, OperationType> SymbolKey_OperationTypeValue_Dictionary = new Dictionary<char, OperationType>()
        {
            { '+', OperationType.Add },
            { '-', OperationType.Subtract },
            { '*', OperationType.Multiply },
            { '/', OperationType.Divide }
        };

        public static Dictionary<OperationType, string> OperationTypeKey_SymbolValue_Dictionary = new Dictionary<OperationType, string>()
        {
            { OperationType.Add      ,"+"    },
            { OperationType.Subtract ,"-"    },
            { OperationType.Multiply ,"*"    },
            { OperationType.Divide   ,"/"    },
            { OperationType.None     ," "    }
        };
    }
}
