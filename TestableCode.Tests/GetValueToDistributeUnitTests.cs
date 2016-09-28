using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace TestableCode.Tests
{
	[TestClass]
	public class GetValueToDistributeUnitTests
	{
		[TestMethod]
		public void ValidValue()
		{
			Entity parentAccount = new Entity("lat_distributeentity");
			parentAccount["lat_valuetodistribute"] = new Money(5000);

			Money expected = new Money(5000);

			Money distributedValue = GoodExample.GetValueToDistribute(parentAccount);

			Assert.AreEqual(distributedValue, expected);
		}

		[TestMethod]
		public void NullValue1()
		{
			Entity parentAccount = new Entity("lat_distributeentity");

			Money distributedValue = GoodExample.GetValueToDistribute(parentAccount);

			Assert.IsNull(distributedValue);
		}

		[TestMethod]
		public void NullValue2()
		{
			Entity parentAccount = new Entity("lat_distributeentity");
			parentAccount["lat_valuetodistribute"] = null;

			Money distributedValue = GoodExample.GetValueToDistribute(parentAccount);

			Assert.IsNull(distributedValue);
		}
	}
}
