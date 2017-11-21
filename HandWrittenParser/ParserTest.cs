using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace HandWrittenParser
{
	[TestFixture]
	internal class ParserTest
	{
		[Test]
		public void CalculatorTest()
		{
			string input = "42 + 5";

			Expression<Func<int>> expression = CalcParserExample.ParseExpression(input);
			int result = expression.Compile()();

			Assert.AreEqual(47, result);
		}

		[Test]
		public void CalculatorFailTest()
		{
			Assert.Catch<Exception>(() =>
			{
				string input = "f + 14";

				Expression<Func<int>> expression = CalcParserExample.ParseExpression(input);
				int result = expression.Compile()();
			}, "unexpected f, expected: Numeric character");
		}
	}
}
