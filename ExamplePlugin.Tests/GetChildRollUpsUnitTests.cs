using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;

namespace ExamplePlugin.Tests
{
	[TestClass]
	public class GetChildRollUpsUnitTests
	{
		#region Class Constructor
		private readonly string _namespaceClassAssembly;
		public GetChildRollUpsUnitTests()
		{
			//[Namespace.class name, assembly name] for the class/assembly being tested
			//Namespace and class name can be found on the class file being tested
			//Assembly name can be found under the project properties on the Application tab
			_namespaceClassAssembly = "ExamplePlugin.RollUp" + ", " + "ExamplePlugin";
		}
		#endregion
		#region Test Initialization and Cleanup
		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void ClassInitialize(TestContext testContext) { }

		// Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void ClassCleanup() { }

		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void TestMethodInitialize() { }

		// Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void TestMethodCleanup() { }
		#endregion

		[TestMethod]
		public void ValidResults()
		{
			//Target
			Entity targetEntity = new Entity { LogicalName = "lat_rollupentity", Id = Guid.NewGuid() };
			targetEntity["lat_parentrollup"] = new EntityReference("lat_rollupentity", Guid.NewGuid());

			//Expected value(s)
			const int expected = 2;

			var serviceMock = new Mock<IOrganizationService>();
			serviceMock = ValidResultsSetup(serviceMock);
			int count = RollUp.GetChildRollUps(new EntityReference(), serviceMock.Object).Entities.Count;

			//Test(s)
			Assert.AreEqual(count, expected);
		}

		/// <summary>
		/// Modify to mock CRM Organization Service actions
		/// </summary>
		/// <param name="serviceMock">The Organization Service to mock</param>
		/// <returns>Configured Organization Service</returns>
		private static Mock<IOrganizationService> ValidResultsSetup(Mock<IOrganizationService> serviceMock)
		{
			EntityCollection queryResult = new EntityCollection();

			Entity rollUp1 = new Entity("lat_rollupentity");
			rollUp1["lat_value"] = 5;
			queryResult.Entities.Add(rollUp1);

			Entity rollUp2 = new Entity("lat_rollupentity");
			rollUp1["lat_value"] = 15;
			queryResult.Entities.Add(rollUp2);

			serviceMock.Setup(t =>
				t.RetrieveMultiple(It.IsAny<QueryExpression>()))
				.ReturnsInOrder(queryResult);

			return serviceMock;
		}
	}
}
