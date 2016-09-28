using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace TestableCode.Tests
{
	[TestClass]
	public class CalculateDistributedValueUnitTests
	{
		[TestMethod]
		public void ValidValue1()
		{
			int count = 1;
			Money valueToDistribute = new Money(5000);
			decimal expectedValue = 5000;

			decimal calculatedValue = GoodExample.CalculateDistributedValue(count, valueToDistribute);

			Assert.AreEqual(calculatedValue, expectedValue);
		}

		[TestMethod]
		public void ValidValue2()
		{
			int count = 4;
			Money valueToDistribute = new Money(5000);
			decimal expectedValue = 1250;

			decimal calculatedValue = GoodExample.CalculateDistributedValue(count, valueToDistribute);

			Assert.AreEqual(calculatedValue, expectedValue);
		}

		[TestMethod]
		public void ValidValue3()
		{
			int count = 32;
			Money valueToDistribute = new Money(5000);
			decimal expectedValue = 156.25m;

			decimal calculatedValue = GoodExample.CalculateDistributedValue(count, valueToDistribute);

			Assert.AreEqual(calculatedValue, expectedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidPluginExecutionException), "We should not have reached this point")]
		public void ZeroValue1()
		{
			int count = 0;
			Money valueToDistribute = new Money(5000);

			decimal calculatedValue = GoodExample.CalculateDistributedValue(count, valueToDistribute);			
		}

		[TestMethod]
		public void ZeroValue2()
		{
			int count = 5;
			Money valueToDistribute = new Money(0);
			decimal expectedValue = 0;

			decimal calculatedValue = GoodExample.CalculateDistributedValue(count, valueToDistribute);

			Assert.AreEqual(calculatedValue, expectedValue);
		}
	}
}
