FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore tools/StreamingProject.LoadTests/StreamingProject.LoadTests.csproj

RUN dotnet publish tools/StreamingProject.LoadTests/StreamingProject.LoadTests.csproj \
	-c Release \
	-o /app/publish \
	--no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0

RUN apt-get update \
	&& apt-get install -y --no-install-recommends ffmpeg \
	&& rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StreamingProject.LoadTests.dll"]
