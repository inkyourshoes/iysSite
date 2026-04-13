FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY iysSite/iysSite.csproj iysSite/
RUN dotnet restore iysSite/iysSite.csproj
COPY iysSite/ iysSite/
RUN dotnet publish iysSite/iysSite.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "iysSite.dll"]
