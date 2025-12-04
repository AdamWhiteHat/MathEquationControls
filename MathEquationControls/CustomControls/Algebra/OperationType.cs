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
        public static Dictionary<char, OperationType> Symbol2OperationTypeDictionary = new Dictionary<char, OperationType>()
        {
            { '+', OperationType.Add },
            { '-', OperationType.Subtract },
            { '*', OperationType.Multiply },
            { '/', OperationType.Divide }
        };

        public static Dictionary<OperationType, string> OperationType2SymbolDictionary = new Dictionary<OperationType, string>()
        {
            { OperationType.Add      ,"+"    },
            { OperationType.Subtract ,"-"    },
            { OperationType.Multiply ,"*"    },
            { OperationType.Divide   ,"/"    },
            { OperationType.None     ," "    }
        };
    }
}
