using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace TestableCode
{
	public class GoodExample : IPlugin
	{
		#region Secure/Unsecure Configuration Setup
		private string _secureConfig = null;
		private string _unsecureConfig = null;

		public GoodExample(string unsecureConfig, string secureConfig)
		{
			_secureConfig = secureConfig;
			_unsecureConfig = unsecureConfig;
		}
		#endregion
		public void Execute(IServiceProvider serviceProvider)
		{
			ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
			IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
			IOrganizationService service = factory.CreateOrganizationService(context.UserId);

			try
			{
				Entity parentDistribute = (Entity)context.InputParameters["Target"];
				Entity postImage = context.PostEntityImages["postImage"];

				EntityCollection results = RetrieveChildRecords(parentDistribute, service);
				Money distributedValue = DetermineDistributedValue(postImage, results);

				//Distribute the value & set the status of each child record
				foreach (Entity distribute in results.Entities)
				{
					distribute["lat_distributedvalue"] = distributedValue;

					distribute["lat_valuestatus"] = DetermineStatusValue(distributedValue);

					service.Update(distribute);
				}

				//Set the status on the parent record
				Entity distributeToUpdate = new Entity("lat_distributeentity") { Id = parentDistribute.Id };
				distributeToUpdate["lat_valuestatus"] = DetermineStatusValue(distributedValue);
				service.Update(distributeToUpdate);
			}
			catch (Exception e)
			{
				throw new InvalidPluginExecutionException(e.Message);
			}
		}

		public static OptionSetValue DetermineStatusValue(Money distributedValue)
		{
			if (distributedValue == null)
				return new OptionSetValue(807990002); //Bad

			if (distributedValue.Value > 1000)
				return new OptionSetValue(807990000); //Good

			if (distributedValue.Value <= 1000 && distributedValue.Value > 500)
				return new OptionSetValue(807990001); //Neutral

			return new OptionSetValue(807990002); //Bad
		}

		public static Money DetermineDistributedValue(Entity parentAccount, EntityCollection results)
		{
			Money valueToDistribute = GetValueToDistribute(parentAccount);

			if (DetermineResultCount(results))
				return null;

			decimal distributedValue = CalculateDistributedValue(results.Entities.Count, valueToDistribute);

			return new Money(distributedValue);
		}

		public static bool DetermineResultCount(EntityCollection results)
		{
			if (results?.Entities == null || results.Entities.Count == 0)
				return true;

			return false;
		}

		public static Money GetValueToDistribute(Entity parentAccount)
		{
			Money valueToDistribute = null;
			object oValueToDistribute;
			bool hasValueToDistribute = parentAccount.Attributes.TryGetValue("lat_valuetodistribute", out oValueToDistribute);
			if (hasValueToDistribute)
				valueToDistribute = (Money)oValueToDistribute;

			return valueToDistribute;
		}

		public static decimal CalculateDistributedValue(int count, Money valueToDistribute)
		{
			if (count == 0)
				throw new InvalidPluginExecutionException("We should not have reached this point");

			return valueToDistribute.Value / count;
		}

		public static EntityCollection RetrieveChildRecords(Entity parentDistribute, IOrganizationService service)
		{
			//Retrieve the child records
			FetchExpression query = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
																  <entity name='lat_distributeentity'>
																	<attribute name='lat_distributeentityid' />
																	<filter type='and'>
																		<condition attribute='lat_parentdistribute' operator='eq' uitype='lat_distributeentity' value='" + parentDistribute.Id + @"'/>
																	</filter>
																</entity>
															</fetch>");

			EntityCollection results = service.RetrieveMultiple(query);

			return results;
		}
	}
}
