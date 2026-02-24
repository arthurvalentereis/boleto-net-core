namespace BoletoNetCore.WebAPI.Extensions
{
    public static class CorsPolicyExtension
    {
        private static string _policyName = "ApiCorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(
                s => s.AddPolicy(_policyName, builder =>
                {
                    builder.WithOrigins("http://letmesee.lenext.com.br",
                                        "https://app.letmesee.com.br",
                                        "http://app.letmesee.com.br",
                                        "https://authinfis.intrainfis.com.br",
                                        "http://authinfis.intrainfis.com.br"
                                        )
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                })
                );

            return services;
        }


        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {

            return app.UseCors(options =>
            {
                options
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin =>
                {
                    // Permite localhost e domínios específicos
                    if (origin.StartsWith("http://localhost"))
                        return true;

                    // Permite domínios do Netlify
                    if (origin.EndsWith(".netlify.app"))
                        return true;

                    // Permite domínios do Netlify
                    if (origin.EndsWith(".lovable.app"))
                        return true;

                    // Permite domínios da sua aplicação
                    var allowedOrigins = new[]
                    {
                        "https://authinfis.intrainfis.com",
                        "http://authinfis.intrainfis.com",
                    };

                    return allowedOrigins.Contains(origin);
                });
            });

        }
    }
}
