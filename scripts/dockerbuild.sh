#!/usr/bin/env bash

SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
  DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
  SOURCE="$(readlink "$SOURCE")"
  [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"

cd $DIR/..

[ -z "$DOTNET_BUILD_CONTAINER_TAG" ] && DOTNET_BUILD_CONTAINER_TAG="dotnetcli-build"

# Build the docker container (will be fast if it is already built)
docker build -t $DOTNET_BUILD_CONTAINER_TAG scripts/docker/

# Run the build in the container
docker run -it --rm -v $(pwd):/opt/code -e DOTNET_BUILD_VERSION=$DOTNET_BUILD_VERSION $DOTNET_BUILD_CONTAINER_TAG /opt/code/build.sh