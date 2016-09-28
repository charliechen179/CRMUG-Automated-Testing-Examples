using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace TestableCode.Tests
{
	[TestClass]
	public class DetermineResultCountUnitTests
	{
		[TestMethod]
		public void ResultsNull()
		{
			EntityCollection results = null;

			bool isEmpty = GoodExample.DetermineResultCount(results);

			Assert.IsTrue(isEmpty);
		}

		[TestMethod]
		public void ResultsEntitiesNullZero()
		{
			EntityCollection results = new EntityCollection();

			bool isEmpty = GoodExample.DetermineResultCount(results);

			Assert.IsTrue(isEmpty);
		}

		[TestMethod]
		public void ResultsEntitiesHasCount()
		{
			EntityCollection results = new EntityCollection();
			results.Entities.Add(new Entity("lat_distributeentity"));

			bool isEmpty = GoodExample.DetermineResultCount(results);

			Assert.IsFalse(isEmpty);
		}
	}
}
