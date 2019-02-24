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

