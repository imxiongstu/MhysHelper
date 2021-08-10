#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MhysHelper.AspNetCore/MhysHelper.AspNetCore.csproj", "MhysHelper.AspNetCore/"]
RUN dotnet restore "MhysHelper.AspNetCore/MhysHelper.AspNetCore.csproj"
COPY . .
WORKDIR "/src/MhysHelper.AspNetCore"
RUN dotnet build "MhysHelper.AspNetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MhysHelper.AspNetCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MhysHelper.AspNetCore.dll"]