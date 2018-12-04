FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY Supperxin.Web.Webcrawler/Supperxin.Web.Webcrawler.csproj Supperxin.Web.Webcrawler/
RUN dotnet restore
COPY . .
WORKDIR /src/Supperxin.Web.Webcrawler
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Supperxin.Web.Webcrawler.dll"]
