FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "exec", "RssNeedle.dll"]
