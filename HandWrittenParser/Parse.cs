using System;
using System.Collections.Generic;
using System.Linq;

namespace HandWrittenParser
{
	public static class Parse
	{
		public static Parser<char> Char(Predicate<char> predicate, string description)
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

		public static Parser<char> IsChar(char target)
		{
			return Char(current => current == target, target.ToString());
		}

		public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "WhiteSpace");
		public static readonly Parser<char> Numeric = Char(char.IsNumber, "Numeric character");
		public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

		public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
		{
			return characters.Select(chs => new string(chs.ToArray()));
		}

		public static Parser<T> Token<T>(this Parser<T> parser)
		{
			return from leading in WhiteSpace.Many()
				from item in parser
				from trailing in WhiteSpace.Many()
				select item;
		}

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

		public static Parser<T> Return<T>(T value)
		{
			return i => Result<T>.Success(value, i);
		}

		public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
		{
			return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
		}

		public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
		{
			return parser.Then(t => Return(convert(t)));
		}

		public static Parser<V> SelectMany<T, U, V>(
			this Parser<T> parser,
			Func<T, Parser<U>> selector,
			Func<T, U, V> projector)
		{
			return parser.Then(t => selector(t).Select(u => projector(t, u)));
		}

		public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
		{
			return parser.Select(r => (IEnumerable<T>)new[] { r });
		}

		public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
		{
			return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
		}
	}
}