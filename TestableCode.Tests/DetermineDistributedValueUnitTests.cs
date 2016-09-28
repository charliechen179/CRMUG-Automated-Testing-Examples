using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace TestableCode.Tests
{
	[TestClass]
	public class DetermineDistributedValueUnitTests
	{
		[TestMethod]
		public void ValidValue()
		{
			Entity parentRecord = new Entity("lat_distributeentity");
			parentRecord["lat_valuetodistribute"] = new Money(5000);

			EntityCollection results = new EntityCollection();
			results.Entities.Add(new Entity("lat_distributeentity"));

			Money expectedValue = new Money(5000);

			Money distributedValue = GoodExample.DetermineDistributedValue(parentRecord, results);

			Assert.AreEqual(distributedValue, expectedValue);
		}
	}
}
