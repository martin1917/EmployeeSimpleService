# SmartwayTask

## Задание  
Web-Сервис сотрудников, сделанный на платформе .Net Core.  
Сервис должен уметь:  
1. Добавлять сотрудников, в ответ должен приходить Id добавленного сотрудника.
2. Удалять сотрудников по Id.
3. Выводить список сотрудников для указанной компании. Все доступные поля.
4. Выводить список сотрудников для указанного отдела компании. Все доступные поля.
5. Изменять сотрудника по его Id. Изменения должно быть только тех полей, которые указаны в запросе.  

Модель сотрудника:
```
{
	Id int
	Name string
	Surname string
	Phone string
	CompanyId int
	Passport {
		Type string
		Number string
	}
	Department {
		Name string
		Phone string
	}
}
```

Все методы должны быть реализованы в виде HTTP запросов в формате JSON.  
БД: любая.  
ORM: Dapper.  

---

Перед запуском нужно поднять базу данных в докере и выполнить sql скрипт:
1. ```docker run --name employee -e POSTGRES_DB=mydb -e POSTGRES_USER=root -e POSTGRES_PASSWORD=root -p 5435:5432 -d postgres``` создание базы
2. ```docker cp .\Data.Postgres\sql\1_up.sql employee:/1_up.sql``` - копирование скрипта (up) в контейнер
3. ```docker cp .\Data.Postgres\sql\1_down.sql employee:/down.sql``` - копирование скрипта (down) в контейнер
4. ```docker exec -u root employee psql mydb root -f ./1_up.sql``` - запуск скрипта для создания таблиц и наполнение данными
