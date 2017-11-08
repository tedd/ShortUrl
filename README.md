# ShortUrl
ASP.Net Core implementation of an URL shortener.

## Version 1
This is version 1. It only has a WebAPI and no webpage.
It comes with near 100% unit test coverage.

Processing time per forward is low. Difficult to measure exactly, 

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
