FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore StreamingProject.sln

RUN dotnet publish StreamingProject.Web/StreamingProject.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends curl ffmpeg \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

RUN useradd --create-home --uid 10001 appuser \
    && mkdir -p \
    /app/wwwroot/hls \
    /home/appuser/.aspnet/DataProtection-Keys

COPY --from=build /app/publish .

RUN chown -R appuser:appuser \
    /app \
    /home/appuser/.aspnet

ENV ASPNETCORE_URLS=http://+:8080

USER appuser

EXPOSE 8080
EXPOSE 1935

ENTRYPOINT ["dotnet", "StreamingProject.Web.dll"]
