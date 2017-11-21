
using System;
using System.Linq.Expressions;

namespace HandWrittenParser
{
	public static class CalcParserExample
	{
		private static readonly Parser<int> NumberToken =
			from number in Parse.Number.Token()
			select int.Parse(number);

		private static readonly Parser<ConstantExpression> Constant =
			from number in NumberToken
			select Expression.Constant(number);

		public static readonly Parser<BinaryExpression> SumExpression =
			from lop in Constant
			from plusOperator in Parse.IsChar('+').Token()
			from rop in Constant
			select Expression.Add(lop, rop);

		public static Expression<Func<int>> ParseExpression(string input)
		{
			var body = SumExpression.Parse(input);
			return Expression.Lambda<Func<int>>(body);
		}
	}
}
