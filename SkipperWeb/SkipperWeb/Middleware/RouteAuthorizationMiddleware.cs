namespace SkipperWeb.Middleware;


// MinimalRouteAuthMiddleware.cs
public class RouteAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, string[]> _protectedRoutes;

    public RouteAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
        
        // Define which routes require which roles
        _protectedRoutes = new Dictionary<string, string[]>
        {
            { "/maintenance", [] },
            // { "/manager", new[] { "Manager", "Admin" } },
            // { "/secure", new[] { "User", "Manager", "Admin" } }
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        // Check if current path starts with any protected route
        var matchedRoute = _protectedRoutes.Keys.FirstOrDefault(route => 
            path?.StartsWith(route) == true);

        if (matchedRoute != null)
        {
            var requiredRoles = _protectedRoutes[matchedRoute];
            var user = context.User;

            // Check if user is authenticated
            if (!user.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authentication required");
                return; // Stop pipeline execution
            }

            // Check if user has required role
            var hasRequiredRole = !requiredRoles.Any() || requiredRoles.Any(role => user.IsInRole(role));
            if (!hasRequiredRole)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Insufficient permissions");
                return; // Stop pipeline execution
            }
        }

        // If we reach here, user is authorized (or route doesn't require auth)
        await _next(context); // Continue to next middleware
    }
}