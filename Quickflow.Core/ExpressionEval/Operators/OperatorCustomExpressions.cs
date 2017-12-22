using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace ExpressionEvaluator.Operators
{
    internal class OperatorCustomExpressions
    {
        /// <summary>
        /// Returns an Expression that accesses a member on an Expression
        /// </summary>
        /// <param name="isFunction">Determines whether the member being accessed is a function or a property</param>
        /// <param name="isCall">Determines whether the member returns void</param>
        /// <param name="le">The expression that contains the member to be accessed</param>
        /// <param name="membername">The name of the member to access</param>
        /// <param name="args">Optional list of arguments to be passed if the member is a method</param>
        /// <returns></returns>
        public static Expression MemberAccess(bool isFunction, bool isCall, Expression le, string membername, List<Expression> args)
        {
            var argTypes = args.Select(x => x.Type);

            Expression instance = null;
            Type type = null;

            var isDynamic = false;
            var isRuntimeType = false;

            if (le.Type.Name == "RuntimeType")
            {
                isRuntimeType = true;
                type = ((Type)((ConstantExpression)le).Value);
            }
            else
            {
                type = le.Type;
                instance = le;
                isDynamic = type.IsDynamic();
            }

            if (isFunction)
            {
                if (isDynamic)
                {
                    var expArgs = new List<Expression> { instance };

                    expArgs.AddRange(args);

                    if (isCall)
                    {
                        var binderMC = Binder.InvokeMember(
                            CSharpBinderFlags.ResultDiscarded,
                            membername,
                            null,
                            type,
                            expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null))
                        );

                        return Expression.Dynamic(binderMC, typeof(void), expArgs);
                    }

                    var binderM = Binder.InvokeMember(
                            CSharpBinderFlags.None,
                            membername,
                            null,
                            type,
                            expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null))
                        );

                    return Expression.Dynamic(binderM, typeof(object), expArgs);
                }
                else
                {
                    var mis = MethodResolution.GetApplicableMembers(type, membername, args);
                    var methodInfo = (MethodInfo)mis[0];

                    if (methodInfo != null)
                    {
                        var parameterInfos = methodInfo.GetParameters();

                        foreach (var parameterInfo in parameterInfos)
                        {
                            var index = parameterInfo.Position;

                            args[index] = TypeConversion.Convert(args[index], parameterInfo.ParameterType);
                        }

                        return Expression.Call(instance, methodInfo, args.ToArray());
                    }

                    var match = MethodResolution.GetExactMatch(type, instance, membername, args) ??
                                MethodResolution.GetParamsMatch(type, instance, membername, args);

                    if (match != null)
                    {
                        return match;
                    }

                }

            }
            else
            {
                if (isDynamic)
                {
                    var binder = Binder.GetMember(
                        CSharpBinderFlags.None,
                        membername,
                        type,
                        new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }
                        );

                    var result = Expression.Dynamic(binder, typeof(object), instance);


                    if (args.Count > 0)
                    {
                        var expArgs = new List<Expression>() { result };

                        expArgs.AddRange(args);

                        var indexedBinder = Binder.GetIndex(
                            CSharpBinderFlags.None,
                            type,
                            expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null))
                            );

                        result =
                            Expression.Dynamic(indexedBinder, typeof(object), expArgs);

                    }

                    return result;
                }
                else
                {
                    Expression exp = null;

                    var propertyInfo = type.GetProperty(membername);
                    if (propertyInfo != null)
                    {
                        exp = Expression.Property(instance, propertyInfo);
                    }
                    else
                    {
                        var fieldInfo = type.GetField(membername);
                        if (fieldInfo != null)
                        {
                            exp = Expression.Field(instance, fieldInfo);
                        }
                    }

                    if (exp != null)
                    {
                        if (args.Count > 0)
                        {
                            return Expression.ArrayAccess(exp, args);
                        }
                        else
                        {
                            return exp;
                        }
                    }
                }


            }

            throw new Exception(string.Format("Member not found: {0}.{1}", le.Type.Name, membername));
        }

        private static readonly Type StringType = typeof(string);
        private static readonly MethodInfo ToStringMethodInfo = typeof(Convert).GetMethod("ToString", new Type[] { typeof(CultureInfo) });


        private static Expression CallToString(Expression instance)
        {
            return Expression.Call(typeof(Convert), "ToString", null, instance, Expression.Constant(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Extends the Add Expression handler to handle string concatenation
        /// </summary>
        /// <param name="le">The left-hand expression</param>
        /// <param name="re">The right-hand expression</param>
        /// <returns></returns>
        public static Expression Add(Expression le, Expression re)
        {
            if (le.Type == StringType || re.Type == StringType)
            {

                if (le.Type != typeof(string)) le = CallToString(le);
                if (re.Type != typeof(string)) re = CallToString(re);
                return Expression.Add(le, re, StringType.GetMethod("Concat", new Type[] { le.Type, re.Type }));
            }
            else
            {
                return Expression.Add(le, re);
            }
        }



        /// <summary>
        /// Returns an Expression that access a 1-dimensional index on an Array expression 
        /// </summary>
        /// <param name="le">The left-hand expression</param>
        /// <param name="re">The right-hand expression</param>
        /// <returns></returns>
        public static Expression ArrayAccess(Expression le, Expression re)
        {
            if (le.Type == StringType)
            {
                var mi = StringType.GetMethod("ToCharArray", new Type[] { });
                le = Expression.Call(le, mi);
            }

            return Expression.ArrayAccess(le, re);
        }

        /// <summary>
        /// Placeholderthat simple returns the left expression
        /// </summary>
        /// <param name="le"></param>
        /// <param name="re"></param>
        /// <returns></returns>
        public static Expression TernarySeparator(Expression le)
        {
            return le;
        }
    }
}