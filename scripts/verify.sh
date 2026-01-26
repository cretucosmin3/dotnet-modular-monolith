#!/usr/bin/env bash
set -e

echo 'Verifying build...'
dotnet --version

echo 'Listing projects:'
dotnet list src/MyProject.* reference

echo 'Attempting dotnet restore/build for solution in repo root...'
dotnet restore || true

dotnet build -v minimal

echo 'Done.'
