FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

COPY *.sln .
COPY src ./src/
COPY tests ./tests/

RUN dotnet restore

WORKDIR /source/src/FitApi.Api

RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "FitApi.Api.dll"]