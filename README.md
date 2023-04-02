# SmartwayTask

Перед запуском нужно поднять базу данных в докере и выполнить sql скрипт:
1. ```docker run --name employee -e POSTGRES_DB=mydb -e POSTGRES_USER=root -e POSTGRES_PASSWORD=root -p 5435:5432 -d postgres``` создание базы
2. ```docker cp .\Data.Postgres\sql\1_up.sql employee:/1_up.sql``` - копирование скрипта (up) в контейнер
3. ```docker cp .\Data.Postgres\sql\1_down.sql employee:/down.sql``` - копирование скрипта (down) в контейнер
4. ```docker exec -u root employee psql mydb root -f ./1_up.sql``` - запуск скрипта для создания таблиц и наполнение данными
