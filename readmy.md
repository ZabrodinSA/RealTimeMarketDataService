# MarcketDataService

## Описание

`MarcketDataService` — это сервис для сбора и хранения данных о тикерах с различных бирж. Он использует `Entity Framework Core` для работы с базой данных и поддерживает кэширование для предотвращения дублирования данных.


## Установка

1. Склонируйте репозиторий:
   ```bash
   git clone https://github.com/ZabrodinSA/RealTimeMarketDataService.git
   cd MarcketDataService
   ```

2. Настройте файл `.env`:
   Укажите параметры подключения к базе данных и настройки модели:
   ```dotenv
   DB_USERNAME=имя_пользователя_postgresql
   DB_PASSWORD=ваш_пароль
   BD_HOST=localhost
   SYMBOL_MAX_LENGTH=20
   EXCHANGE_NAME_MAX_LENGTH=20
   PRICE_PRECISION=18
   PRICE_SCALE=8
   VOLUME_PRECISION=18
   VOLUME_SCALE=8
   CACHE_TTL_PER_MINUTES=1
   ```

3. Установите зависимости:
   ```bash
   dotnet restore
   ```

4. Запустите приложение:
   ```bash
   dotnet run
   ```

## Инструкция по добавлению новых бирж

## Инструкция по добавлению новой биржи (только `appsettings.json`)

1. **Откройте файл `appsettings.json`**:
   Файл находится в корне проекта.

2. **Добавьте настройки для новой биржи**:
   В соответствующий раздел добавьте параметры для новой биржи. Например:
   ```json
   {
      "Exchanges": [
         {
            "Name": "Binance",
            "Url": "ws://localhost:5017/ws/binance"
         },
         {
            "Name": "new_name",
            "Url": "new_url"
         }
      ],
   }

3. **Перезапустите приложение**:
   После внесения изменений в `appsettings.json`, перезапустите приложение, чтобы новые настройки вступили в силу.
