using System;
using System.Collections.Generic;
using System.Linq;

namespace HandWrittenParser
{
	/// <summary>
	/// Constais pasres
	/// </summary>
	public static class Parse
	{
		// predicate current == "a"
		// input: "abc" -> result (success, value: "a", remainder: "bc")
		public static Parser<char> Char(Func<char, bool> predicate, string description)
		{
			return i =>
			{
				if (!i.AtEnd)
				{
					return predicate(i.Current) 
						? Result<char>.Success(i.Current, i.GetNext()) 
						: Result<char>.Failure($"unexpected {i.Current}, expected: {description}");
				}

				return Result<char>.Failure("Unexpected end of string");
			};
		}
		// target = a
		// input: "abc" -> result (success, value: "a", remainder: "bc")
		public static Parser<char> Char(char target)
		{
			return Char(current => current == target, target.ToString());
		}

		// input: " abc" -> result (success, value: " ", remainder: "abc")
		public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "WhiteSpace");
		// input: "11 + 2" -> result (success, value: "1", remainder: "1 + 2")
		public static readonly Parser<char> Numeric = Char(char.IsNumber, "Numeric character");
		// input: "112 + 2" -> result (success, value: "112", remainder: " + 2")
		public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

		// maps Parse<IEnumerable<char>> to Parse<string>
		public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
		{
			return characters.Select(chs => new string(chs.ToArray()));
		}

		// input parser Parse Numbers
		// input: " 123 + 12" -> result (success, value: "123", remainder: "+ 12")
		public static Parser<T> Token<T>(this Parser<T> parser)
		{
			return from leading in WhiteSpace.Many()
				   from item in parser
				   from trailing in WhiteSpace.Many()
				   select item;
		}

		// parses while parser<T> is successoful
		public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
		{
			return i =>
			{
				var remainder = i;
				var result = new List<T>();
				var r = parser(i);

				while (r.WasSuccessful)
				{
					result.Add(r.Value);
					remainder = r.Remainder;
					r = parser(remainder);
				}

				return Result<IEnumerable<T>>.Success(result, remainder);
			};
		}

		// parse('-') then parse('Number')
		// input: "-1 + 5" -> result (success, value: "-1", remainder: "+ 12")
		public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
		{
			return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
		}

		//maps T to Parser<T>
		public static Parser<T> Return<T>(T value)
		{
			return i => Result<T>.Success(value, i);
		}

		//maps/selects result from T to U.
		public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
		{
			return parser.Then(t => Return(convert(t)));
		}

		// like LINQ SelectMany 
		public static Parser<V> SelectMany<T, U, V>(
			this Parser<T> parser,
			Func<T, Parser<U>> selector,
			Func<T, U, V> projector)
		{
			return parser.Then(t => selector(t).Select(u => projector(t, u)));
		}

		// parses<T> and returns it as IEnumerable<T>
		public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
		{
			return parser.Select(r => (IEnumerable<T>)new[] { r });
		}

		// parses<T> once and then parsers while successful
		public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
		{
			return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
		}
	}
}