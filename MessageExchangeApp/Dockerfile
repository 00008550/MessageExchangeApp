# Use the official ASP.NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Create Logs directory
RUN mkdir -p /app/Logs

# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MessageExchangeApp.csproj", "./"]
RUN dotnet restore "./MessageExchangeApp.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MessageExchangeApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageExchangeApp.csproj" -c Release -o /app/publish

# Use the ASP.NET runtime to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageExchangeApp.dll"]
