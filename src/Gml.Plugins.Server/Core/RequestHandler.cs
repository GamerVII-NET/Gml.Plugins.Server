namespace Gml.Plugins.Server.Core;

public static class RequestHandler
{


    public static WebApplication RegisterEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/plugins", PluginsHandler.GetPlugins);
        app.MapGet("/api/v1/plugins/{name}/{version}", PluginsHandler.GetPlugin);
        
        return app;
    }
}