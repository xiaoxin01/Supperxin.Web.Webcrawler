#!/bin/bash

dir=$(dirname $0)

echo "build project at $dir"

docker-compose -f $dir/docker-compose.yml \
  -f $dir/docker-compose.tag.yml \
  build

echo "start project"
docker-compose -f $dir/docker-compose.yml \
  -f $dir/docker-compose.tag.yml \
  up
