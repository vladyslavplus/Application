FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Evently.Domain/Evently.Domain.csproj Evently.Domain/
COPY Evently.Application/Evently.Application.csproj Evently.Application/
COPY Evently.Infrastructure/Evently.Infrastructure.csproj Evently.Infrastructure/
COPY Evently.Api/Evently.Api.csproj Evently.Api/

RUN dotnet restore Evently.Api/Evently.Api.csproj

COPY . .
RUN dotnet publish Evently.Api/Evently.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Evently.Api.dll"]
