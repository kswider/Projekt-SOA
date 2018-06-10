FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# Expose ports
EXPOSE 80
EXPOSE 443

# Toulbar2
RUN printf "\ndeb http://ftp.debian.org/debian unstable main contrib non-free" >> /etc/apt/sources.list
RUN printf "\ndeb-src http://ftp.debian.org/debian unstable main contrib non-free\n" >> /etc/apt/sources.list
RUN apt update && apt -y install toulbar2

CMD ["dotnet", "watch", "--verbose", "run"]
