#!/bin/bash
git pull
docker compose build
docker compose up --force-recreate --no-deps web -d

