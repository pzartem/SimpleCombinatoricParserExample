using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SpracheCalc
{
	[TestFixture]
	class Tests
	{
		public void TestParser()
		{
			var str = "42 + 5 - 7";
			var func = SpracheCalcParser.ParseExpression(str);

			var result = func.Compile()();

			Assert.AreEqual(40, result);
		}
	}
}
