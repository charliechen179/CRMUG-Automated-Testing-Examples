using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace ExamplePlugin
{
	public class RollUp : IPlugin
	{
		#region Secure/Unsecure Configuration Setup
		private string _secureConfig = null;
		private string _unsecureConfig = null;

		public RollUp(string unsecureConfig, string secureConfig)
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
				Entity postImage = context.PostEntityImages["postImage"];

				EntityReference parentRollUp = postImage.GetAttributeValue<EntityReference>("lat_parentrollupid");
				if (parentRollUp == null)
					return;

				EntityCollection results = GetChildRollUps(parentRollUp, service);

				int total = SumValues(results);

				tracer.Trace("The total is: " + total);

				Entity updateRollUp = new Entity("lat_rollupentity");
				updateRollUp.Id = parentRollUp.Id;
				updateRollUp["lat_childvaluetotal"] = total;
				service.Update(updateRollUp);
			}
			catch (Exception e)
			{
				throw new InvalidPluginExecutionException(e.Message);
			}
		}

		public static int SumValues(EntityCollection results)
		{
			int total = 0;
			foreach (Entity rollUp in results.Entities)
			{
				total += rollUp.GetAttributeValue<int>("lat_value");
			}

			return total;
		}

		public static EntityCollection GetChildRollUps(EntityReference parentRollUp, IOrganizationService service)
		{
			QueryExpression query = new QueryExpression
			{
				EntityName = "lat_rollupentity",
				ColumnSet = new ColumnSet("lat_value"),
				Criteria = new FilterExpression
				{
					Conditions =
					{
						new ConditionExpression
						{
							AttributeName = "lat_parentrollupid",
							Operator = ConditionOperator.Equal,
							Values = { parentRollUp.Id }
						}
					}
				}
			};

			return service.RetrieveMultiple(query);
		}
	}
}
