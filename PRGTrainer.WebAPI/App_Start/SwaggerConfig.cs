using System.Web.Http;
using WebActivatorEx;
using PRGTrainer.WebAPI;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace PRGTrainer.WebAPI
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c => { c.SingleApiVersion("v1", "PRGTrainer.WebAPI"); })
                .EnableSwaggerUi(c => { });
        }
    }
}
