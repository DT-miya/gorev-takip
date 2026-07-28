using Microsoft.AspNetCore.Http;

namespace TaskBoard.Api.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientIpAddress(this HttpContext context)
    {
        // 1. Proxy / Load Balancer arkasındaysa gerçek IP'yi alır
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedHeader))
        {
            return forwardedHeader.Split(',')[0].Trim();
        }

        // 2. Doğrudan bağlantıdaki IP adresi
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
         // Localhost IPv6 (::1) ise bunu IPv4 (127.0.0.1) biçimine çevir
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                return remoteIp.MapToIPv4().ToString();
            }
            
            if (remoteIp.ToString() == "::1")
            {
                return "127.0.0.1";
            }

            return remoteIp.ToString();
        }

        return "127.0.0.1";
    }
}