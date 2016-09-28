using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace TestableCode.Tests
{
	[TestClass]
	public class DetermineStatusValueUnitTests
	{
		[TestMethod]
		public void GoodStatus1()
		{
			Money distributedValue = new Money(5000);

			OptionSetValue expectedValue = new OptionSetValue(807990000);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void GoodStatus2()
		{
			Money distributedValue = new Money(1001);

			OptionSetValue expectedValue = new OptionSetValue(807990000);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void NeutralStatus1()
		{
			Money distributedValue = new Money(1000);

			OptionSetValue expectedValue = new OptionSetValue(807990001);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void NeutralStatus2()
		{
			Money distributedValue = new Money(750);

			OptionSetValue expectedValue = new OptionSetValue(807990001);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void NeutralStatus3()
		{
			Money distributedValue = new Money(501);

			OptionSetValue expectedValue = new OptionSetValue(807990001);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void BadStatus1()
		{
			Money distributedValue = new Money(500);

			OptionSetValue expectedValue = new OptionSetValue(807990002);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}

		[TestMethod]
		public void BadStatus2()
		{
			Money distributedValue = null;

			OptionSetValue expectedValue = new OptionSetValue(807990002);

			OptionSetValue valueStatus = GoodExample.DetermineStatusValue(distributedValue);

			Assert.AreEqual(valueStatus, expectedValue);
		}
	}
}
