FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore tools/StreamingProject.HttpSignalRLoadTests/StreamingProject.HttpSignalRLoadTests.csproj

RUN dotnet publish tools/StreamingProject.HttpSignalRLoadTests/StreamingProject.HttpSignalRLoadTests.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StreamingProject.HttpSignalRLoadTests.dll"]
