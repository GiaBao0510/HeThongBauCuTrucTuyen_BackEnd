using Microsoft.AspNetCore.Builder;

namespace BackEnd.src.web_api.MapEndPoints
{
    public static class MapEndpoints
    {
        public static WebApplication MapUserEndPoint(this WebApplication app){
            app.MapGet(pattern: "User", () => {});
            return app;
        }

        public static WebApplication MapAdminEndPoint(this WebApplication app){
            app.MapGet(pattern: "Admin", () => {});
            return app;
        }
    }
}