#!/bin/bash

dir=$(dirname $0)
docker-compose -f $dir/docker-compose.yml \
  -f $dir/docker-compose.override.yml \
  up
