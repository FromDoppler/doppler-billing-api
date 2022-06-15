FROM mcr.microsoft.com/dotnet/sdk:6.0.202-bullseye-slim AS restore
WORKDIR /src
COPY ./*.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

FROM restore AS build
COPY . .
RUN dotnet build -c Release

FROM build AS test
RUN dotnet test /p:CollectCoverage=true

FROM build AS publish
RUN dotnet publish "Billing.API/Billing.API.csproj" -c Release -o /app/publish

# When it is running in a W2016 host, it uses mcr.microsoft.com/dotnet/core/aspnet:2.1.8-nanoserver-sac2016
FROM mcr.microsoft.com/dotnet/aspnet:6.0.6-bullseye-slim AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
COPY ./wwwroot_extras/ /app/wwwroot/
ARG version=unknown
RUN echo $version > /app/wwwroot/version.txt
ENTRYPOINT ["dotnet", "Billing.API.dll"]
