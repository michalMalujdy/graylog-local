# Graylog

## Startup
Start all necessary services with `docker-compose up`.

## Dashboard url
http://localhost:9000

## Credentials
- username: admin
- password: admin

## Connect your .NET+Serilog app to the Graylog input
## GELF
## Your app is running on the local machine
Add `WriteTo` section to the Serilog configuration. Port `5050` for GELF is exposed from the Graylog container, so it's accessible through localhost.
```json
  "Serilog": {
    ...
    "WriteTo": [
      {
        "Name": "Graylog",
        "Args": {
          "hostnameOrAddress": "localhost",
          "port": "5050",
          "transportType": "Http"
        }
      }
    ]
  }
```
### Your app is running in a docker container
The difference is that your app will have an isolated network, so it won't be able to access the Graylog container directly without an additional configuration. This `docker-compose` exposes `graylog-network` bridge driver, so you can add that network to your `docker-compose` under your service like so:
```yaml
version: "3"
services:
  my-app:
    ...
    networks:
      - my-network
networks:
  my-network:
    external:
      name: "graylog-network"
```
## Filebeat
Logs for the filebeat are watched and automatically picked up from `C:/ProgramData/Scalepoint/logs/*` path.