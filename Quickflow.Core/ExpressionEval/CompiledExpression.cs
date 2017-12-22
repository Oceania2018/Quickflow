using System;
using System.Linq.Expressions;

namespace ExpressionEvaluator
{

    /// <summary>
    /// Creates compiled expressions with return values that are of type T
    /// </summary>
    public class CompiledExpression<T> : ExpressionCompiler
    {
        private Func<T> _compiledMethod = null;
        private Action _compiledAction = null;

        public CompiledExpression()
        {
            Parser = new Parser { TypeRegistry = TypeRegistry };
        }

        public CompiledExpression(string expression)
        {
            Parser = new Parser(expression) { TypeRegistry = TypeRegistry };
        }

        public Func<T> Compile(bool isCall = false)
        {
            if (Expression == null) Expression = WrapExpression(BuildTree(), false);
            return Expression.Lambda<Func<T>>(Expression).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that returns void
        /// </summary>
        /// <returns></returns>
        public Action CompileCall()
        {
            if (Expression == null) Expression = BuildTree(null, true);
            return Expression.Lambda<Action>(Expression).Compile();
        }


        /// <summary>
        /// Compiles the expression to a function that takes an object as a parameter and returns an object
        /// </summary>s
        /// <returns></returns>
        public Action<U> ScopeCompileCall<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam, true);
            return Expression.Lambda<Action<U>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }


        public Func<object, T> ScopeCompile()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<dynamic, T>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        public Func<U, T> ScopeCompile<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<U, T>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
            _compiledAction = null;
        }

        public T Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return _compiledMethod();
        }

        public void Call()
        {
            if (_compiledAction == null) _compiledAction = CompileCall();
            _compiledAction();
        }

        public object Global
        {
            set
            {
                Parser.Global = value;
            }
        }

    }

    /// <summary>
    /// Creates compiled expressions with return values that are cast to type Object 
    /// </summary>
    public class CompiledExpression : ExpressionCompiler
    {
        private Func<object> _compiledMethod = null;
        private Action _compiledAction = null;

        public CompiledExpression()
        {
            Parser = new Parser();
            Parser.TypeRegistry = TypeRegistry;

        }

        public CompiledExpression(string expression)
        {
            Parser = new Parser(expression);
            Parser.TypeRegistry = TypeRegistry;
        }

        /// <summary>
        /// Compiles the expression to a function that returns an object
        /// </summary>
        /// <returns></returns>
        public Func<object> Compile()
        {
            if (Expression == null) Expression = WrapExpression(BuildTree());
            return Expression.Lambda<Func<object>>(Expression).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that returns void
        /// </summary>
        /// <returns></returns>
        public Action CompileCall()
        {
            if (Expression == null) Expression = BuildTree(null, true);
            return Expression.Lambda<Action>(Expression).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that takes an object as a parameter and returns an object
        /// </summary>
        /// <returns></returns>
        public Func<object, object> ScopeCompile()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<dynamic, object>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that takes an object as a parameter and returns an object
        /// </summary>
        /// <returns></returns>
        public Action<object> ScopeCompileCall()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam, true);
            return Expression.Lambda<Action<dynamic>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that takes an object as a parameter and returns an object
        /// </summary>s
        /// <returns></returns>
        public Action<U> ScopeCompileCall<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam);
            return Expression.Lambda<Action<U>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        /// <summary>
        /// Compiles the expression to a function that takes an typed object as a parameter and returns an object
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public Func<U, object> ScopeCompile<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<U, object>>(Expression, new ParameterExpression[] { scopeParam }).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
            _compiledAction = null;
        }

        public object Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return _compiledMethod();
        }

        public void Call()
        {
            if (_compiledAction == null) _compiledAction = CompileCall();
            _compiledAction();
        }

        public object Global
        {
            set
            {
                Parser.Global = value;
            }
        }

    }
}
