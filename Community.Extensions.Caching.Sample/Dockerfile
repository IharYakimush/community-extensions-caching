FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Comminity.Extensions.Caching.Sample/Comminity.Extensions.Caching.Sample.csproj Comminity.Extensions.Caching.Sample/
RUN dotnet restore Comminity.Extensions.Caching.Sample/Comminity.Extensions.Caching.Sample.csproj
COPY . .
WORKDIR /src/Comminity.Extensions.Caching.Sample
RUN dotnet build Comminity.Extensions.Caching.Sample.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Comminity.Extensions.Caching.Sample.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Comminity.Extensions.Caching.Sample.dll"]
