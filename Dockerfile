# Define build arguments for GIT branch, GIT commit, and datetime


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files for all layers (1-web, 2-services, 3-data)
COPY ./TeletekstBotHangfire/TeletekstBotHangfire.csproj ./TeletekstBotHangfire/

# Restore dependencies for all layers
RUN dotnet restore ./TeletekstBotHangfire/TeletekstBotHangfire.csproj

# Copy the entire solution
COPY . .

# Build the solution
RUN dotnet build "./TeletekstBotHangfire/TeletekstBotHangfire.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish

RUN dotnet publish "./TeletekstBotHangfire/TeletekstBotHangfire.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
RUN apt-get update && apt-get install -y wget apt-transport-https software-properties-common \
    && wget -q https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y powershell \
    && rm packages-microsoft-prod.deb

COPY --from=publish /app/publish .
# Make sure the playwright browsers are installed
RUN pwsh /app/playwright.ps1 install-deps
RUN pwsh /app/playwright.ps1 install
