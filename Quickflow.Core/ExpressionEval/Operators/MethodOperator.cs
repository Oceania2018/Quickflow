using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionEvaluator.Operators
{
    internal class MethodOperator : Operator<Func<bool, bool, Expression, string, List<Expression>, Expression>>
    {
        public MethodOperator(string value, int precedence, bool leftassoc,
                              Func<bool, bool, Expression, string, List<Expression>, Expression> func)
            : base(value, precedence, leftassoc, func)
        {
        }
    }

    internal class TernarySeparatorOperator : Operator<Func<Expression, Expression>>
    {
        public TernarySeparatorOperator(string value, int precedence, bool leftassoc,
                              Func<Expression, Expression> func)
            : base(value, precedence, leftassoc, func)
        {
        }
    }
}