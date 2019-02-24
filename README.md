A tool to crawl web site.

## support types:

- [x] Html rendered web site. Get metadata from html directly
- [x] Json data from api.

## Configuration files

You can get configuration files from:

    Supperxin.Web.Webcrawler/Configurations/

There are some demo configurations for:

1. v2ex hot topic and job.
2. readhub
3. 创业邦快讯(cyzone)

## 4 steps to crawl data.

1. copy or create setting file

Choose a setting file from Supperxin.Web.Webcrawler/Configurations, or create your own.

    cp Supperxin.Web.Webcrawler/Configurations/appsettings.cyzone.json Supperxin.Web.Webcrawler/appsettings.json

2. build project

make file: docker-compose.tag.yml, then run change the tag

    cp docker-compose.override.yml docker-compose.tag.yml

```yaml
version: '3.4'

services:
  supperxin.web.webcrawler:
    image: supperxin.web.webcrawler:[tag]
```

3. add save result to (option)

```
"SaveTo": {
    "Type": "HttpPipline",
    "ProcessUrl": [Your url]
},
```

4. start crawler

    bash start.sh
