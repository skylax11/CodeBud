using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class SameSiteCookieManager : ICookieManager
{
    private readonly ChunkingCookieManager _chunkingCookieManager = new ChunkingCookieManager();

    public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
    {
        _chunkingCookieManager.AppendResponseCookie(context, key, value, options);

        if (context.Response.Headers.ContainsKey("Set-Cookie"))
        {
            var cookies = context.Response.Headers.GetValues("Set-Cookie").ToList();

            for (int i = 0; i < cookies.Count; i++)
            {
                if (cookies[i].Contains(key))
                {
                    cookies[i] = AddSameSite(cookies[i]);
                }
            }

            context.Response.Headers.SetValues("Set-Cookie", cookies.ToArray());
        }
    }


    public string GetRequestCookie(IOwinContext context, string key)
    {
        return _chunkingCookieManager.GetRequestCookie(context, key);
    }

    public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
    {
        _chunkingCookieManager.DeleteCookie(context, key, options);
    }

    private string AddSameSite(string cookie)
    {
        if (!cookie.Contains("SameSite"))
        {
            cookie += "; SameSite=None; Secure";
        }
        return cookie;
    }
}
