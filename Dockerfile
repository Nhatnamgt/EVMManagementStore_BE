# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy toàn bộ mã nguồn
COPY . .

# Restore và publish project chính
RUN dotnet restore "EVMManagementStore/EVMManagementStore.csproj"
RUN dotnet publish "EVMManagementStore/EVMManagementStore.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EVMManagementStore.dll"]
