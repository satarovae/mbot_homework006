# ToDoListBot: ДЗ «Подключение ORM» (linq2db + PostgreSQL)

## Запуск

1. Создать БД и таблицы (PostgreSQL):
   sql
   CREATE DATABASE "ToDoList";
   
   Затем выполнить в БД ToDoList скрипты ToDoListDb.sql (таблицы, ключи, индексы).
2. Задать токен бота (в коде его нет):
   - Windows (PowerShell) $env:TELEGRAM_BOT_TOKEN="123:ABC"
   - Linux/macOS export TELEGRAM_BOT_TOKEN=123:ABC
3. Строка подключения по умолчанию указана в Program.cs:
   Host=localhost;Port=5432;Database=ToDoList;Username=postgres;Password=postgres.
4. dotnet run --project ToDoListBot

## Что сделано по пунктам задания

| Пункт | Где |
|---|---|
| Пакеты linq2db.PostgreSQL (+ linq2db, Npgsql) | ToDoListBot.csproj |
| Анемичные сущности без конструкторов, все свойства get; set; | Core/Entities |
| Модели с [Table], [PrimaryKey], [Column], [Association] | Core/DataAccess/Models |
| ToDoDataContext : DataConnection со свойствами ToDoUsers, ToDoLists, ToDoItems | Infrastructure/DataAccess/ToDoDataContext.cs |
| IDataContextFactory<TDataContext> и DataContextFactory | Infrastructure/DataAccess |
| ModelMapper (все 6 методов) | Infrastructure/DataAccess/ModelMapper.cs |
| SqlToDoRepository (с LoadWith), SqlToDoListRepository, SqlUserRepository | Infrastructure/DataAccess |
| SQL-репозитории вместо File-реализаций, connectionString | Program.cs |