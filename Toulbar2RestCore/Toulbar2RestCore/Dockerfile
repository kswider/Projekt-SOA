FROM microsoft/dotnet:2.1-sdk AS build

# Expose ports
EXPOSE 8080

RUN apt-get update && apt-get install -y gcc

# Add user and group
RUN useradd -r -u 300 -m -g users -c "Normal user" t2r

# Install su-exec to get down from root
RUN wget https://raw.githubusercontent.com/ncopa/su-exec/master/su-exec.c \
    && gcc -o /usr/local/bin/su-exec su-exec.c \
    && rm su-exec.c

# Install fixuid (https://github.com/boxboat/fixuid) to map host user id with container 't2r' user id
RUN USER=t2r && \
    GROUP=users && \
    wget -qO- https://github.com/boxboat/fixuid/releases/download/v0.3/fixuid-0.3-linux-amd64.tar.gz | tar -C /usr/local/bin -xzf - && \
    chown root:root /usr/local/bin/fixuid && \
    chmod 4755 /usr/local/bin/fixuid && \
    mkdir -p /etc/fixuid && \
    printf "user: $USER\ngroup: $GROUP\n" > /etc/fixuid/config.yml

# Toulbar2
RUN printf "\ndeb http://ftp.debian.org/debian unstable main contrib non-free" >> /etc/apt/sources.list
RUN printf "\ndeb-src http://ftp.debian.org/debian unstable main contrib non-free\n" >> /etc/apt/sources.list
RUN apt update && apt -y install toulbar2

COPY entrypoint.sh /

USER t2r:t2r

WORKDIR /app

ENTRYPOINT ["/entrypoint.sh"]
CMD ["dotnet", "watch", "--verbose", "run"]
