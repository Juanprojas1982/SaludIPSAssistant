# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/SaludIPSAssistant.WebApi/SaludIPSAssistant.WebApi.csproj", "src/SaludIPSAssistant.WebApi/"]
COPY ["src/SaludIPSAssistant.Application/SaludIPSAssistant.Application.csproj", "src/SaludIPSAssistant.Application/"]
COPY ["src/SaludIPSAssistant.Domain/SaludIPSAssistant.Domain.csproj", "src/SaludIPSAssistant.Domain/"]
COPY ["src/SaludIPSAssistant.Infrastructure/SaludIPSAssistant.Infrastructure.csproj", "src/SaludIPSAssistant.Infrastructure/"]

RUN dotnet restore "src/SaludIPSAssistant.WebApi/SaludIPSAssistant.WebApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/SaludIPSAssistant.WebApi"
RUN dotnet build "SaludIPSAssistant.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SaludIPSAssistant.WebApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install timezone data for proper date/time handling
RUN apt-get update && apt-get install -y tzdata && rm -rf /var/lib/apt/lists/*

# Set timezone to Colombia
ENV TZ=America/Bogota

COPY --from=publish /app/publish .

# Create a non-root user
RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --gid 1001 dotnet

# Change ownership of the app directory
RUN chown -R dotnet:dotnet /app
USER dotnet

EXPOSE 8080

ENTRYPOINT ["dotnet", "SaludIPSAssistant.WebApi.dll"]