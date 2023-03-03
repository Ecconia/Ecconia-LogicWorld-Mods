using System;
using System.Linq.Expressions;

namespace WireTracer.Server
{
	public static class DelegateConstructor
	{
		public static Func<T, R> createGetterForProperty<T, R>(string propertyName)
		{
			var parameter = Expression.Parameter(typeof(T), "obj");
			var property = Expression.Property(parameter, propertyName);
			var convert = Expression.Convert(property, typeof(R));
			var lambda = Expression.Lambda(typeof(Func<T, R>), convert, parameter);
			return (Func<T, R>) lambda.Compile();
		}

		public static Func<T, R> createGetterForField<T, R>(string fieldName)
		{
			var parameter = Expression.Parameter(typeof(T));
			var field = Expression.Field(parameter, fieldName);
			var convert = Expression.Convert(field, typeof(R));
			return Expression.Lambda<Func<T, R>>(convert, parameter).Compile();
		}
	}
}
