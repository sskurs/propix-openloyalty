Full Staging Stack (PostgreSQL)
Includes:
- Admin (placeholder .NET Web API)
- Worker (.NET worker service)
- docker-compose.yml wiring postgres, redis, kafka, zookeeper, kafka-ui, admin, worker
- migration scripts & seed SQL

How to run:
1. Install Docker & Docker Compose
2. From this folder run: docker compose up --build
3. Admin API: http://localhost:5000
   Worker health: http://localhost:8085/healthz

