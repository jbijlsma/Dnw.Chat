# Introduction

Simplistic chat broadcast example that allows horizontal scaling of the asp.net core web app. It uses Redis PubSub / RabbitMq and Server Side Events. 

Note that the asp.net web app serves as both the front-end and api. A background service is used to consume all the chat messages.  

# Testing locally

You can configure either Redis or RabbitMq using the PUB_SUB_TYPE environment variable. For allowed values see the PubSubType enum.

To use with Redis start a redis instance locally (in docker):

```
docker run -dp 6379:6379 redis
```

To use RabbitMq instead run:

```
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

And for Kafka use:

```
docker-compose -f ./kafka-docker-compose.yml up -d
```

Use your IDE to run the Dnw.Chat project or use the command line like below:

```
cd ./Dnw.Chat

export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=https://localhost:5002
export PUB_SUB_TYPE=Redis
dotnet run --no-launch-profile
```

Set the environment variables using your IDE run configuration or prior to calling 'dotnet run' like shown above.

To test, open multiple browser windows / tabs on https://localhost:5002. Each browser window / tab represents a unique random user and chat messages are broadcasted to all other connected users, regardless to which dnw-chat web app they are connected to.  

# Deploy to local k8s cluster (KinD)

The instructions below apply to running natively on apple silicon (M1 pro). To run on x64 you need to:

- Update the ./k8s/redis.yml file and use a x64 image. The current image used is: arm64v8/redis:latest
- Update the ./Dnw.Chat/Dockerfile. In the build phase you should not run 'dotnet restore' and 'dotnet publish' with the -r linux-musl-arm64 flag. Also the publish base image should not be an arm64 image. The current image is: aspnet:6.0-alpine-arm64v8.

Use the ./k8s/creat_kind_cluster.sh script to create a cluster:

- It creates a KinD cluster with a local image registry
- It pre-loads some images
- It deploys an nginx instance to the cluster

Once you have a k8s cluster, use ./k8s/deploy_local.sh to deploy the Dnw.Chat application to the cluster:

- It builds & publishes the image for the Dnw.Chat app and pushes it to the local image registry
- It deploys a redis instance and 2 dnw-chat web app instances to the cluster using helm
- It creates an ingress resource that configures nginx to load balance between the 2 dnw-chat pods (round-robin)

In ./k8s/helm/templates/chat.yml you can specify how many pods you want with replicas (the default is 2):

```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ .Release.Name }}-deployment
spec:
  replicas: 2
```

Run the script like so:

```
cd ./k8s \
./deploy_local.sh \

kubectl get po --all-namespaces (verify everything is running)
```

# Local k8s testing

Open one or more browser windows / tabs at: http://localhost/chat

First find out the names of the dnw-chat pods:

```
kubctl get po -n dnw-chat
```

Then tail the logs for the pods in separate terminal windows:

```
kubectl logs -n dnw-chat -f {pod name}
```

You should note 2 things:

- When you type a chat message in any browser window / tab the message appears in all other open browser windows / tabs.
- In the console logs you can see which dnw-chat pod received the post message from the    

NOTE:

Configuring kubectl shell autocomplete makes all this a lot easier:

https://kubernetes.io/docs/tasks/tools/included/optional-kubectl-configs-zsh

# Good to know

Http/2 and Http/3 is supported since .net core 3 but not on all linux distributions. Also TLS 1.2 is needed.

See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/http2?view=aspnetcore-6.0

