# ============================================
# Stage 1 - Build
# ============================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project file
COPY Gateway.csproj .

# Restore packages
RUN dotnet restore

# Copy source code including ocelot.json
COPY . .

# Publish application
RUN dotnet publish Gateway.csproj \
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

# Explicitly copy Ocelot configuration
COPY --from=build /src/ocelot.json /app/ocelot.json

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Gateway.dll"]