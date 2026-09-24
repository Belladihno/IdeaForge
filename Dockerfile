# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["IdeaForge.API/IdeaForge.API.csproj", "IdeaForge.API/"]
COPY ["IdeaForge.Application/IdeaForge.Application.csproj", "IdeaForge.Application/"]
COPY ["IdeaForge.Domain/IdeaForge.Domain.csproj", "IdeaForge.Domain/"]
COPY ["IdeaForge.Infrastructure/IdeaForge.Infrastructure.csproj", "IdeaForge.Infrastructure/"]
RUN dotnet restore "IdeaForge.API/IdeaForge.API.csproj"

COPY . .
RUN dotnet publish "IdeaForge.API/IdeaForge.API.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT; Program.cs binds Kestrel to it. No fixed ports here.
ENTRYPOINT ["dotnet", "IdeaForge.API.dll"]
