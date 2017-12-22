using System.Linq.Expressions;

namespace ExpressionEvaluator.Operators
{
    internal interface IOperator
    {
        string Value { get; set; }
        int Precedence { get; set; }
        int Arguments { get; set; }
        bool LeftAssoc { get; set; }
        ExpressionType ExpressionType { get; set; }
    }
}