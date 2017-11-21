using System;

namespace HandWrittenParser
{
	internal static class ResultHelper
	{
		public static Result<U> IfSuccess<T, U>(this Result<T> result, Func<Result<T>, Result<U>> next)
		{
			return result.WasSuccessful 
				? next(result) 
				: Result<U>.Failure(result.Message);
		}
	}
}