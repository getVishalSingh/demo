FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/EmpApi.Api/EmpApi.Api.csproj src/EmpApi.Api/
RUN dotnet restore src/EmpApi.Api/EmpApi.Api.csproj
COPY src/ src/
RUN dotnet publish src/EmpApi.Api/EmpApi.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir /data && chown $APP_UID /data
ENV ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__Default="Data Source=/data/emp.db"
USER $APP_UID
EXPOSE 8080
VOLUME /data
ENTRYPOINT ["dotnet", "EmpApi.Api.dll"]
