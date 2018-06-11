# toulbar2 REST

## Prerequisites

1. Go to **certs** directory and rename **key.pfx.example** to **key.pfx**.
2. Add following line to **hosts** file (/etc/hosts in Linux):
~~~
127.0.1.1   t2r.localhost
~~~

## Running container

1. Run command:

    $ docker-compose up --build

2. Wait for server start with message as following:

~~~
soa                 | Now listening on: http://[::]:8080          
soa                 | Now listening on: https://[::]:8081
soa                 | Application started. Press Ctrl+C to shut down.
~~~

3. Open https://t2r.localhost

## Notes

All files are watched and every changed is reloaded within few seconds.
