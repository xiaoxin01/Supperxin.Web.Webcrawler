A tool to crawl web site.

You can view the crawled result from this site:

[http://listen.supperxin.com/News](http://listen.supperxin.com/News)

## support types

- [x] Html rendered web site. Get metadata from html directly
- [x] Json data from api.

## Functions

- [x] Configurable. Only need to create an new configuration file for new web site.
- [x] List page crawl. Can crawl items from list page directly.
- [x] Iteration page crawl. Can crawl page by specify logic.
  - eg. crawl http://site?p=1 to http://site?p=10
- [x] Make operations to crawled results.
  - eg. change https://www.v2ex.com/t/538237#reply5 to https://www.v2ex.com/t/538237
- [x] Result cache. Don't crawl the same result by key(maybe url).

## Configuration files

You can get configuration files from:

    Supperxin.Web.Webcrawler/Configurations/

There are some demo configurations for:

1. v2ex hot topic and job.
2. readhub
3. 创业邦快讯(cyzone)

## 3 steps to crawl data.

1. copy or create configuration file

Choose a setting file from Supperxin.Web.Webcrawler/Configurations, or create your own.

    cp Supperxin.Web.Webcrawler/Configurations/appsettings.cyzone.json Supperxin.Web.Webcrawler/appsettings.json

Create file: docker-compose.tag.yml, then change the tag

The tag can be set to the crawled site name.

    cp docker-compose.override.yml docker-compose.tag.yml

```yaml
version: '3.4'

services:
  supperxin.web.webcrawler:
    image: supperxin.web.webcrawler:[tag]
```

2. add save result to (option)

```
"SaveTo": {
    "Type": "HttpPipline",
    "ProcessUrl": [Your url]
},
```

3. start crawler

    bash start.sh
