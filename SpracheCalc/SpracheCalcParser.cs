using System;
using System.Linq.Expressions;
using Sprache;

namespace SpracheCalc
{

	public static class SpracheCalcParser
	{
		private static Parser<ExpressionType> CreateOperator(string op, ExpressionType type)
		{
			return Parse.String(op).Token().Return(type);
		}

		private static readonly Parser<ExpressionType> Add = CreateOperator("+", ExpressionType.Add);
		private static readonly Parser<ExpressionType> Subtract = CreateOperator("-", ExpressionType.Subtract);

		private static readonly Parser<Expression> Operand =
			from number in Parse.Number.Token()
			select Expression.Constant(int.Parse(number));

		private static readonly Parser<Expression> MainExpression =
			Parse.ChainOperator(
				Add.Or(Subtract),
				Operand,
				Expression.MakeBinary);

		public static Expression<Func<int>> ParseExpression(string input)
		{
			var body = MainExpression.Parse(input);
			return Expression.Lambda<Func<int>>(body);
		}
	}
}
