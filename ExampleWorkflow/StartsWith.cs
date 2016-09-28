using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace ExampleWorkflow
{
	public class StartsWith : CodeActivity
	{
		[RequiredArgument]
		[Input("String To Search")]
		public InArgument<string> StringToSearch { get; set; }

		[RequiredArgument]
		[Input("Search For")]
		public InArgument<string> SearchFor { get; set; }

		[RequiredArgument]
		[Input("Case Sensitive")]
		[Default("false")]
		public InArgument<bool> CaseSensitive { get; set; }

		[OutputAttribute("Starts With String")]
		public OutArgument<bool> StartsWithString { get; set; }

		protected override void Execute(CodeActivityContext executionContext)
		{
			try
			{
				string stringToSearch = StringToSearch.Get(executionContext);
				string searchFor = SearchFor.Get(executionContext);
				bool caseSensitive = CaseSensitive.Get(executionContext);

				bool startsWithString = stringToSearch.StartsWith(searchFor,
					(caseSensitive) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);

				StartsWithString.Set(executionContext, startsWithString);
			}
			catch (Exception e)
			{
				throw new InvalidPluginExecutionException(e.Message);
			}
		}
	}
}
