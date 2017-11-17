# ShortUrl
ASP.Net Core C¤ implementation of an URL shortener much like "tinyurl.com", "bitl.y", "goo.gl", "ow.ly", etc...

## Version 1
This is version 1. It only has a WebAPI and no webpage.
It comes with near 100% unit test coverage.

If is fast, responds within a few milliseconds.

## Config


Change appsettings.json:

 - Configure SQL server
 - Set up access tokens for consumers of webservice.
<pre><code>"AccessTokens": {
"SOME-SECRET-TOKEN": {
   "Enbled": true,
   "Admin":  true 
}</code></pre>

 - Admin means it can initialize/upgrade database and request metadata for urls it did not create.

## WebAPI
### Create
<pre><code>PUT /Admin/Create
{
	"AccessToken": "SOME-SECRET-TOKEN",
	"Url": "http://www.stackoverflow.com/",
	"MetaData": "Put anything here, for example json",
	"DateTime": "2018-01-01 15:30"
}</code></pre>

### Get (metadata)
<pre><code>GET /Admin/Get/key?AccessToken=SOME-SECRET-TOKEN
{
	"AccessToken": "SOME-SECRET-TOKEN",
	"Url": "http://www.stackoverflow.com/",
	"MetaData": "Put anything here, for example json",
	"DateTime": "2018-01-01 15:30"
}</code></pre>

### Initialize database (Migrations)
<pre><code>GET /Admin/Upgrade/SOME-SECRET-TOKEN</code></pre>

### Visit URL
Simply visit URL returned from Create operation to be forwarded.

# HTTPS
You may want to add automatic forwarding to HTTPS with support for letsencrypt:

https://blog.tedd.no/2017/05/09/iis-redirect-http-to-https-but-allow-lets-encrypt/
