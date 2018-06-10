# toulbar2 REST

## Prerequisites

Go to **certs** directory and rename **key.pfx.example** to **key.pfx**.

## Running container

1. Run command:

    $ docker-compose up

2. Wait for server start with message as following:

~~~
soa                 | Now listening on: https://0.0.0.0:8080
soa                 | Application started. Press Ctrl+C to shut down.
~~~

3. Open https://0.0.0.0:8080

## Notes

All files are watched and every changed is reloaded within few seconds.
