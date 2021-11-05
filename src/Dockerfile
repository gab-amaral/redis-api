FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT Development

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["Redis.API/Redis.API.csproj", "Redis.API/"]
RUN dotnet restore "Redis.API/Redis.API.csproj"
COPY . .
RUN dotnet publish "Redis.API/Redis.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Redis.API.dll"]
