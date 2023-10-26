# SimbirGOAPI

Задание полуфинального этапа - Волга IT

---

## Запуск в докере

```powershell
docker-compose up
```

#### URL: http://localhost:5000/swagger/index.html

---

## Запуск без докера

1. Восстановить базу данных из файла "SimbirGODb.backup"
2. Отредактировать конфиг "SimbirGOAPI\SimbirGOAPI\bin\Release\net7.0\appsettings.json"

```json
"ConnectionStrings": {
    "SimbirGODb": "host=localhost:5432; username=username; password=password; Database=database"
},
```

3. запустить "SimbirGOAPI\SimbirGOAPI\bin\Release\net7.0\SimbirGOAPI.exe"

#### URL: http://localhost:5000/swagger/index.html  

---

## Запуск через CLI

Отредактировать конфиг "SimbirGOAPI\SimbirGOAPI\appsettings.json"

```json
"ConnectionStrings": {
    "SimbirGODb": "host=localhost:5432; username=username; password=password; Database=database"
},
```

```powershell
cd .\SimbirGOAPI\
dotnet run
```

#### URL: http://localhost:5091/swagger/index.html