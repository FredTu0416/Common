using System;
using System.Linq.Expressions;
using System.Linq;

namespace CommonService
{
    public class Assembly
    {
        public static Func<T> CreateInstance<T>() where T : IExpressionGenerate
        {
            NewExpression exp = Expression.New(typeof(T));
            var instance = Expression.Lambda<Func<T>>(exp);
            return instance.Compile();
        }

        public static Func<TIn,TOut> CreateInstance<TIn,TOut>() where TOut : IExpressionGenerate
        {
            var constructors = typeof(TOut).GetConstructors();
            var constructorInfo = constructors.FirstOrDefault(a => a.GetParameters().Length == 1);
            if (constructorInfo == null)
                throw new ArgumentNullException("未找到参数个数为1的构造方法");

            ParameterExpression p1 = Expression.Parameter(typeof(TIn));
            NewExpression exp = Expression.New(constructorInfo, p1);
            var instance = Expression.Lambda<Func<TIn,TOut>>(exp, p1);
            return instance.Compile();
        }

        public static Func<TIn1,TIn2, TOut> CreateInstance<TIn1,TIn2, TOut>() where TOut : IExpressionGenerate
        {
            var constructors = typeof(TOut).GetConstructors();
            var constructorInfo = constructors.FirstOrDefault(a => a.GetParameters().Length == 2);
            if (constructorInfo == null)
                throw new ArgumentNullException("未找到参数个数为2的构造方法");

            ParameterExpression p1 = Expression.Parameter(typeof(TIn1));
            ParameterExpression p2 = Expression.Parameter(typeof(TIn2));
            NewExpression exp = Expression.New(constructorInfo, p1, p2);
            var instance = Expression.Lambda<Func<TIn1,TIn2, TOut>>(exp, p1,p2);
            return instance.Compile();
        }
    }
}
