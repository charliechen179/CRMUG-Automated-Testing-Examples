using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace ExamplePlugin
{
	public class Concatenate : IPlugin
	{
		#region Secure/Unsecure Configuration Setup
		private string _secureConfig = null;
		private string _unsecureConfig = null;

		public Concatenate(string unsecureConfig, string secureConfig)
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
				Entity entity = (Entity)context.InputParameters["Target"];
				Entity preImage = context.PreEntityImages["preImage"];

				EntityReference parentRollUp = preImage.GetAttributeValue<EntityReference>("lat_parentrollupid");
				if (parentRollUp == null)
					return;

				EntityCollection results = GetChildRollUps(parentRollUp, service);

				int total = SumValues(results, entity.Id);
				total += entity.GetAttributeValue<int>("lat_value");

				entity["lat_name"] = entity.GetAttributeValue<int>("lat_value") + "/" + total;

				UpdateChildren(results, entity.Id, total, service);
			}
			catch (Exception e)
			{
				throw new InvalidPluginExecutionException(e.Message);
			}
		}

		public static void UpdateChildren(EntityCollection results, Guid currentId, int total, IOrganizationService service)
		{
			foreach (Entity rollUp in results.Entities)
			{
				if (rollUp.Id == currentId)
					continue;

				Entity updateRollup = new Entity("lat_rollupentity");
				updateRollup.Id = rollUp.Id;
				updateRollup["lat_name"] = rollUp.GetAttributeValue<int>("lat_value") + "/" + total;
				service.Update(updateRollup);
			}
		}

		public static int SumValues(EntityCollection results, Guid currentId)
		{
			int total = 0;
			foreach (Entity rollUp in results.Entities)
			{
				if (rollUp.Id == currentId)
					continue;

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
