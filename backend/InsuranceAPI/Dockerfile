# ============================================
# Stage 1 - Build
# ============================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY InsuranceAPI.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish InsuranceAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================
# Stage 2 - Runtime
# ============================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "InsuranceAPI.dll"]