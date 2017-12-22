using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionEvaluator.Tokens;

namespace ExpressionEvaluator.Operators
{
    internal class OpFuncArgs
    {
        public Queue<Token> TempQueue { get; set; }
        public Stack<Expression> ExprStack { get; set; }
        public Token T { get; set; }
        public IOperator Op { get; set; }
        public List<Expression> Args { get; set; }
        public Expression ScopeParam { get; set; }
        public List<string> Types { get; set; }
    }
}