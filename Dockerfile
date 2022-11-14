# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /source

# Restore nuget dependencies
COPY ./*.csproj .
RUN dotnet restore -r linux-musl-arm64

# Copy other files
COPY . .
RUN dotnet publish -c release -o /app --self-contained false --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-arm64v8
# FROM mcr.microsoft.com/dotnet/runtime:6.0.11-jammy-arm64v8
WORKDIR /app
COPY --from=build /app .

# Configure kestrel
ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT [ "./Dnw.Chat" ]