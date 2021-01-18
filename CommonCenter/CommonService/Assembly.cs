using System;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace CommonService
{
    public class Assembly
    {
        public static Func<T> CreateInstance<T>() where T : class
        {
            NewExpression exp = Expression.New(typeof(T));
            var instance = Expression.Lambda<Func<T>>(exp);
            return instance.Compile();
        }

        public static Func<TIn,TOut> CreateInstance<TIn,TOut>() where TOut : class
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

        public static Func<TIn1,TIn2, TOut> CreateInstance<TIn1,TIn2, TOut>() where TOut : class
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

        public static Action<Object, Object> SetValue(PropertyInfo property)
        {
            var param_instance = Expression.Parameter(typeof(Object));
            var param_value = Expression.Parameter(typeof(Object));
            var body_instance = Expression.Convert(param_instance, property.DeclaringType);
            var body_value = Expression.Convert(param_value, property.PropertyType);
            var body_call = Expression.Call(body_instance, property.GetSetMethod(), body_value);
            return Expression.Lambda<Action<Object, Object>>(body_call, param_instance, param_value).Compile();
        }

        public static Action<TInstance, TValue> SetValue<TInstance,TValue>(PropertyInfo property)
        {
            var param_instance = Expression.Parameter(typeof(TInstance));
            var param_value = Expression.Parameter(typeof(TValue));
            var body_instance = Expression.Convert(param_instance, property.DeclaringType);
            var body_value = Expression.Convert(param_value, property.PropertyType);
            var body_call = Expression.Call(body_instance, property.GetSetMethod(), body_value);
            return Expression.Lambda<Action<TInstance, TValue>>(body_call, param_instance, param_value).Compile();
        }

        public static Func<object, object> GetValue(PropertyInfo property)
        {
            throw new Exception();
        }
    }
}
