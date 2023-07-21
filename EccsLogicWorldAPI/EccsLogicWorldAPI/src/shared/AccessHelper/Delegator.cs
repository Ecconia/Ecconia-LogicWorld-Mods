using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	/*
	 * Code inspired from:
	 * - https://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo
	 * - https://stackoverflow.com/questions/6158768/c-sharp-reflection-fastest-way-to-update-a-property-value
	 * - https://stackoverflow.com/questions/1600712/a-constructor-as-a-delegate-is-it-possible-in-c
	 * Although it is obvious, once understood.
	 */
	public static class Delegator
	{
		public static Func<RESULT> createStaticFieldGetter<RESULT>(FieldInfo field)
		{
			var fieldExpression = Expression.Field(null, field);
			return Expression.Lambda<Func<RESULT>>(fieldExpression).Compile();
		}
		
		public static Action<VALUE> createStaticFieldSetter<VALUE>(FieldInfo field)
		{
			var valueExpression = Expression.Parameter(field.FieldType, "value");
			var fieldExpression = Expression.Field(null, field);
			var assignExpression = Expression.Assign(fieldExpression, valueExpression);
			return Expression.Lambda<Action<VALUE>>(assignExpression, valueExpression).Compile();
		}
		
		public static Func<INSTANCE, RESULT> createFieldGetter<INSTANCE, RESULT>(FieldInfo field)
		{
			var instanceParameter = Expression.Parameter(typeof(INSTANCE));
			Expression instanceToSearch = instanceParameter;
			if(typeof(INSTANCE) != field.DeclaringType)
			{
				instanceToSearch = Expression.TypeAs(instanceParameter, field.DeclaringType);
			}
			var fieldExpression = Expression.Field(instanceToSearch, field);
			return Expression.Lambda<Func<INSTANCE, RESULT>>(fieldExpression, instanceParameter).Compile();
		}
		
		public static Action<INSTANCE, VALUE> createFieldSetter<INSTANCE, VALUE>(FieldInfo field)
		{
			var valueExpression = Expression.Parameter(typeof(VALUE));
			var instanceParameter = Expression.Parameter(typeof(INSTANCE));
			Expression instanceToSearch = instanceParameter;
			if(typeof(INSTANCE) != field.DeclaringType)
			{
				instanceToSearch = Expression.TypeAs(instanceParameter, field.DeclaringType);
			}
			var fieldExpression = Expression.Field(instanceToSearch, field);
			var assignExpression = Expression.Assign(fieldExpression, valueExpression);
			return Expression.Lambda<Action<INSTANCE, VALUE>>(assignExpression, instanceParameter, valueExpression).Compile();
		}
		
		public static Action<INSTANCE, VALUE> createPropertySetter<INSTANCE, VALUE>(PropertyInfo property)
		{
			var setter = property.SetMethod;
			var valueExpression = Expression.Parameter(typeof(VALUE));
			var instanceParameter = Expression.Parameter(typeof(INSTANCE));
			Expression instanceToSearch = instanceParameter;
			if(typeof(INSTANCE) != setter.DeclaringType)
			{
				instanceToSearch = Expression.TypeAs(instanceParameter, setter.DeclaringType);
			}
			var callExpression = Expression.Call(instanceToSearch, setter, valueExpression);
			return Expression.Lambda<Action<INSTANCE, VALUE>>(callExpression, instanceParameter, valueExpression).Compile();
		}
		
		public static Func<INSTANCE, RESULT> createPropertyGetter<INSTANCE, RESULT>(PropertyInfo property)
		{
			var getter = property.GetMethod;
			var instanceParameter = Expression.Parameter(typeof(INSTANCE));
			Expression instanceToSearch = instanceParameter;
			if(typeof(INSTANCE) != getter.DeclaringType)
			{
				instanceToSearch = Expression.TypeAs(instanceParameter, getter.DeclaringType);
			}
			var callExpression = Expression.Call(instanceToSearch, getter);
			return Expression.Lambda<Func<INSTANCE, RESULT>>(callExpression, instanceParameter).Compile();
		}
		
		//TODO: Make generic to allow a random amount of arguments (or at least copy paste to make use of any amount).

		public static Action<A1> createStaticMethodCall<A1>(MethodInfo methodInfo)
		{
			var a1 = Expression.Parameter(typeof(A1), "a1");
			var callExpression = Expression.Call(null, methodInfo, a1);
			return Expression.Lambda<Action<A1>>(callExpression, a1).Compile();
		}
		
		public static Func<A1, A2, A3, A4, A5, RESULT> createObjectInitializer<A1, A2, A3, A4, A5, RESULT>(ConstructorInfo constructor)
		{
			var a1 = Expression.Parameter(typeof(A1), "a1");
			var a2 = Expression.Parameter(typeof(A2), "a2");
			var a3 = Expression.Parameter(typeof(A3), "a3");
			var a4 = Expression.Parameter(typeof(A4), "a4");
			var a5 = Expression.Parameter(typeof(A5), "a5");
			var lambda = Expression.Lambda<Func<A1, A2, A3, A4, A5, RESULT>>(
				Expression.New(constructor, a1, a2, a3, a4, a5), 
				a1, a2, a3, a4, a5);
			return lambda.Compile();
		}
	}
}
