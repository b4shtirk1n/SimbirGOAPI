# SimbirGOAPI

Задание полуфинального этапа - Волга IT.  
Без использования медиатора и паттерна репозиторий(лишняя абстракция)

---

## Запуск в докере

```powershell
docker-compose up
```

#### URL: http://localhost:5000/swagger/index.html
#### URL (Для сервера с настроенным nginx): http://localhost/swagger/index.html

---

## Запуск без докера

1. Восстановить базу данных из файла "SimbirGODb.backup"
2. Отредактировать конфиг "SimbirGOAPI\SimbirGOAPI\bin\Release\net7.0\appsettings.json"

```json
"ConnectionStrings": {
    "SimbirGODb": "host=localhost:5432; username=username; password=password; Database=database"
},
```

3. Запустить "SimbirGOAPI\SimbirGOAPI\bin\Release\net7.0\SimbirGOAPI.exe"

#### URL: http://localhost:5000/swagger/index.html  

---

## Запуск через CLI

1. Восстановить базу данных из файла "SimbirGODb.backup"
2. Отредактировать конфиг "SimbirGOAPI\SimbirGOAPI\appsettings.json"

```json
"ConnectionStrings": {
    "SimbirGODb": "host=localhost:5432; username=username; password=password; Database=database"
},
```

3. Запустить в консоли

```powershell
cd .\SimbirGOAPI\
dotnet run -c release
```

#### URL: http://localhost:5000/swagger/index.html