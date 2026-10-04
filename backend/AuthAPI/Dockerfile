# ============================================
# Stage 1 - Build
# ============================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project file first
# This improves Docker layer caching
COPY AuthAPI.csproj .

# Restore NuGet packages
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish AuthAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================
# Stage 2 - Runtime
# ============================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# ASP.NET Core container port
EXPOSE 8080

# Explicitly tell ASP.NET Core to listen on 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Start application
ENTRYPOINT ["dotnet", "AuthAPI.dll"]