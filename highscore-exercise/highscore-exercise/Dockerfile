#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["highscore-exercise/highscore-exercise.csproj", "highscore-exercise/"]
RUN dotnet restore "highscore-exercise/highscore-exercise.csproj"
COPY . .
WORKDIR "/src/highscore-exercise"
RUN dotnet build "highscore-exercise.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "highscore-exercise.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "highscore-exercise.dll"]