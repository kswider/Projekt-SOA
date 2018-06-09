FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80
RUN printf "\ndeb http://ftp.debian.org/debian unstable main contrib non-free" >> /etc/apt/sources.list
RUN printf "\ndeb-src http://ftp.debian.org/debian unstable main contrib non-free\n" >> /etc/apt/sources.list
RUN apt update && apt -y install toulbar2

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Toulbar2RestCore.sln ./
COPY Toulbar2RestCore/Toulbar2RestCore.csproj Toulbar2RestCore/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Toulbar2RestCore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Toulbar2RestCore.dll"]
