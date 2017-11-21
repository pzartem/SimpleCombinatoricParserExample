using System;

namespace HandWrittenParser
{
	/// <summary>
	/// Specifies Parser signature.
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="input"></param>
	/// <returns></returns>
	public delegate Result<TValue> Parser<TValue>(Input input);

	public static class ParserExtensions
	{
		public static T Parse<T>(this Parser<T> parser, string input)
		{
			var result = parser(new Input(input));

			if (result.WasSuccessful)
				return result.Value;

			throw new Exception(result.Message);
		}
	}
}