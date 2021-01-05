using Swashbuckle.Swagger;
using System;
using System.Web.Http.Description;

namespace TRMDataManager.App_Start
{
	public class AuthTokenOperation : IDocumentFilter
	{
		public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
		{
			throw new NotImplementedException();
		}
	}
}