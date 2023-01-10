# Build image
#FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-arm64v8 AS build
#FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy as build
FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy as build
WORKDIR /source

# Restore nuget dependencies
COPY ./*.csproj .
RUN dotnet restore -r linux-musl-arm64

# Copy other files
COPY . .
RUN dotnet publish -c release -o /app --self-contained false --no-restore

# Runtime stage
#FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine-arm64v8
#FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy
#FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy
#FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy
WORKDIR /app
COPY --from=build /app .

# Install kafka librdkafka-dev using apk alpine package manager
#RUN apk update
#RUN apk add --upgrade librdkafka-dev

# Configure kestrel
ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT [ "./Dnw.Chat.Api" ]