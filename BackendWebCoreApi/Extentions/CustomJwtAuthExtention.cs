using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace BackendWebCoreApi.Extentions
{
    public static class CustomJwtAuthExtention
    {
        public static void AddCustomJwtAuth( this IServiceCollection service, ConfigurationManager conf)
        {
            service.AddAuthentication(o =>
            {
                // for search token in the right place:
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // for write bearer then token in post man for example
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;   // for go to login if unauthoriz
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;   // other schemes in services have to work on jwtbearerschema
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false; // if true, it runs on just https
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()  // validate parameters in configuration file : Issuer, Audience ..
                {
                    ValidateIssuer = true,
                    ValidIssuer = conf["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWT:SecretKey"]))
                };
            });
        }
    }
}
