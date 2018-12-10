using System;

namespace Lucinq.Lucene30.UnitTests
{
	class TestHelpers
	{

		public static string GetDateString(DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMdd-hhMMss-mmm");
		}
	}
}
