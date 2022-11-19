#!/bin/bash

# Define variables
RELEASE_NAME=dnw-chat

# Create the new cluster with a private container / image registry
#echo "Create new KinD cluster"
#. ./create_kind_cluster.sh

# Preload 3rd party images
docker pull mcr.microsoft.com/dotnet/sdk:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/sdk:6.0-alpine

docker pull mcr.microsoft.com/dotnet/aspnet:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/aspnet:6.0-alpine

docker pull arm64v8/redis:latest
kind load docker-image arm64v8/redis:latest

docker pull rabbitmq:3.11-management
kind load docker-image rabbitmq:3.11-management

docker pull confluentinc/cp-zookeeper:7.3.0
kind load docker-image confluentinc/cp-zookeeper:7.3.0

docker pull confluentinc/cp-kafka:7.3.0
kind load docker-image confluentinc/cp-kafka:7.3.0

# Build local image, tag it and push it to the local registry
TAG="localhost:5001/$RELEASE_NAME:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.Chat.Api/Dockerfile ../Dnw.Chat.Api/
docker push $TAG

# Install app into k8s cluster
helm upgrade "$RELEASE_NAME" ./helm --install --namespace "$RELEASE_NAME" --create-namespace

# Restart the deployment
DEPLOYMENT="deployment/$RELEASE_NAME-deployment"
echo "DEPLOYMENT=$DEPLOYMENT"
kubectl rollout restart "deployment/$RELEASE_NAME-deployment" -n "$RELEASE_NAME"