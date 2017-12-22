using System;

namespace ExpressionEvaluator.Tokens
{
    internal class Token
    {
        public object Value { get; set; }
        public bool IsIdent { get; set; }
        public bool IsOperator { get; set; }
        public bool IsType { get; set; }
        public Type Type { get; set; }
        public int ArgCount { get; set; }
        public int Ptr { get; set; }
        public bool IsCast { get; set; }
        public bool IsScope { get; set; }
        public bool IsFunction { get; set; }
        public bool IsCall { get; set; }
    }
}