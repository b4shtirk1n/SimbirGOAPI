FROM mcr.microsoft.com/dotnet/aspnet:7.0 as base
ENV ASPNETCORE_URLS=http://+:5000
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . /src
WORKDIR /src
RUN ls
RUN dotnet build "SimbirGOAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimbirGOAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimbirGOAPI.dll"]