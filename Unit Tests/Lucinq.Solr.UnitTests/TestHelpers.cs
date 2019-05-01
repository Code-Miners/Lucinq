namespace Lucinq.Solr.UnitTests
{
    using System;

    class TestHelpers
	{

		public static string GetDateString(DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMdd-hhMMss-mmm");
		}
	}
}
