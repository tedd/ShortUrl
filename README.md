# ShortUrl
ASP.Net Core C¤ implementation of an URL shortener much like "tinyurl.com", "bitl.y", "goo.gl", "ow.ly", etc...

## Version 1
This is version 1. It only has a WebAPI and no webpage.
It comes with near 100% unit test coverage.

## Setup
### Webserver
Solution is written in ASP.Net Core 2.0.

 1. Set up solution by publishing it to a folder and pointing a webserver at it. 
 For example: [Guide for setting up ASP.Net Core on IIS](https://docs.microsoft.com/en-us/aspnet/core/publishing/iis?tabs=aspnetcore2x)
 2. Change `appsettings.json` to configure a database. You should also change the `UgradePassword`.
 3. Visit `http://yoururl.com/Admin/Upgrade/$$UPGRADEPWD$$` where `$$UPGRADEPWD$$` is whatever you set UpgradePassword to. This will run EF Migrations and create your database.
 4. If the token database is empty, a default token is created. Check `ShortUrlAccessToken` table.
 Tokens are required for services to be able to create shorturls on system.

Note: Admin means it can initialize/upgrade database and request metadata for urls it did not create.

## WebAPI
### Create
#### Request
```json
POST /Admin/Create
{
	"AccessToken": "$$TESTTOKEN$!!$CHANGEME$$",
	"Url": "http://mysite.com/$URL$",
	"MetaData": "Put anything here, for example json",
	"Expires": "2018-01-01 15:30"
}
```

Note:
* `Expires` is optional, and should be given in UTC.
* `$URL$` in the URL will be replaced with the shorturl key. You can use this to pass reference for target solutin to go back into ShortUrl system to pick up metadata. This way metadata can be passed in URL without going via user browser.

#### Response
```json
{
	"success": true,
	"key": "abc123",
	"shortUrl": "http://myshorturlservice.com/abc123",
	"url": "http://www.stackoverflow.com/",
	"metaData": "Put anything here, for example json",
	"expiresUtc": "2018-01-01 15:30"
}
```

### Get (metadata)
```json
GET /Admin/Get/key?AccessToken=$$TESTTOKEN$!!$CHANGEME$$
{
	"accessToken": "$$TESTTOKEN$!!$CHANGEME$$"",
	"url": "http://www.stackoverflow.com/$URL$",
	"metaData": "Put anything here, for example json",
	"expiresUtc": "2018-01-01 15:30"
}
```

### Initialize database (Migrations)
<pre><code>GET /Admin/Upgrade/$$UPGRADEPWD$$</code></pre>

### Visit URL
Simply visit ShortUrl returned from Create operation to be forwarded to Url.

# HTTPS
You may want to add automatic forwarding to HTTPS, maybe even with support for letsencrypt:

https://blog.tedd.no/2017/05/09/iis-redirect-http-to-https-but-allow-lets-encrypt/
