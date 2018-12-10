using System;

namespace Lucinq.AzureSearch.UnitTests
{
	class TestHelpers
	{

		public static string GetDateString(DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMdd-hhMMss-mmm");
		}
	}
}
