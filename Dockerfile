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
COPY --from=publish /app/publish .
