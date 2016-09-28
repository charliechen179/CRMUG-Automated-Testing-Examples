using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace TestableCode
{
	public class BadExample : IPlugin
	{
		#region Secure/Unsecure Configuration Setup
		private string _secureConfig = null;
		private string _unsecureConfig = null;

		public BadExample(string unsecureConfig, string secureConfig)
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

				//Distribute the value & set the status of each child record
				foreach (Entity distribute in results.Entities)
				{
					distribute["lat_distributedvalue"] = new Money(postImage.GetAttributeValue<Money>("lat_valuetodistribute").Value / results.Entities.Count);

					if (distribute.GetAttributeValue<Money>("lat_distributedvalue").Value > 1000)
					{
						distribute["lat_valuestatus"] = new OptionSetValue(807990000); //Good
					}
					else if (distribute.GetAttributeValue<Money>("lat_distributedvalue").Value < 1000 && distribute.GetAttributeValue<Money>("lat_distributedvalue").Value > 500)
					{
						distribute["lat_valuestatus"] = new OptionSetValue(807990001); //Neutral
					}
					else
					{
						distribute["lat_valuestatus"] = new OptionSetValue(807990002); //Bad
					}

					service.Update(distribute);
				}

				//Set the status on the parent record
				Entity distributeToUpdate = new Entity("lat_distributeentity") { Id = parentDistribute.Id };
				if (results.Entities[0].GetAttributeValue<Money>("lat_distributedvalue").Value > 1000)
				{
					distributeToUpdate["lat_valuestatus"] = new OptionSetValue(807990000); //Good
				}
				else if (results.Entities[0].GetAttributeValue<Money>("lat_distributedvalue").Value < 1000 && results.Entities[0].GetAttributeValue<Money>("lat_distributedvalue").Value > 500)
				{
					distributeToUpdate["lat_valuestatus"] = new OptionSetValue(807990001); //Neutral
				}
				else
				{
					distributeToUpdate["lat_valuestatus"] = new OptionSetValue(807990002); //Bad
				}

				service.Update(distributeToUpdate);
			}
			catch (Exception e)
			{
				throw new InvalidPluginExecutionException(e.Message);
			}
		}
	}
}
