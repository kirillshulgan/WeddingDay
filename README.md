# WeddingDay

## Запуск локально
1. `cp source/secrets/.env.example source/secrets/.env` и заполнить значения.
2. `dotnet run --project source/WeddingDay.AppHost`

## Структура
- `docs/` — документация
- `source/secrets/` — `.env` (не в git) и `.env.example`
- `source/WeddingDay.AppHost` — Aspire-оркестрация
- `source/WeddingDay.Gateway` — YARP + OIDC/BFF
- `source/WeddingDay.ServiceDefaults` — OpenTelemetry, health, resilience, общие константы