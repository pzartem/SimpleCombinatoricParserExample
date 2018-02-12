
using System;
using System.Linq.Expressions;

namespace HandWrittenParser
{	
	public static class CalcParserExample
	{
		private static readonly Parser<ConstantExpression> Constant =
			from number in Parse.Number.Token()
			select Expression.Constant(int.Parse(number));

		private static readonly Parser<char> AddOperator =
			from addOperator in Parse.Char('+').Token()
			select addOperator;

		private static readonly Parser<BinaryExpression> SumExpression =
			from lop in Constant
			from addOperator in AddOperator
			from rop in Constant
			select Expression.Add(lop, rop);

		public static Expression<Func<int>> ParseExpression(string input)
		{
			var body = SumExpression.Parse(input);
			return Expression.Lambda<Func<int>>(body);
		}
	}
}
