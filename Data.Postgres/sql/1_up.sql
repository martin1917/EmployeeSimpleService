CREATE TABLE departments(
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(30),
    Phone VARCHAR(30)
);

CREATE TABLE passports(
    Id SERIAL PRIMARY KEY,
    Type VARCHAR(30),
    Number VARCHAR(30)
);

CREATE TABLE employees(
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(30),
    Surname VARCHAR(30),
    Phone VARCHAR(30),
    CompanyId INTEGER,
    PassportId INTEGER REFERENCES passports(Id) UNIQUE,
    DepartmentId INTEGER REFERENCES passports(Id)
);

INSERT INTO departments (Id, Name, Phone) VALUES 
(1, 'test', '123'),
(2, 'game', '465'),
(3, 'sales', '852'),
(4, 'marketing', '986');

INSERT INTO passports (Id, Type, Number) VALUES
(1, 'type_1', '123456'),
(2, 'type_2', '789654'),
(3, 'type_3', '1475'),
(4, 'type_4', '26790'),
(5, 'type_5', '961436'),
(6, 'type_6', '09755');

INSERT INTO employees (Name, Surname, Phone, CompanyId, PassportId, DepartmentId) VALUES
('petya', 'ivanov', '789', 1, 1, 1),
('ivan', 'petrov', '159', 1, 2, 2),
('denis', 'potyr', '7777', 1, 3, 2),
('vlad', 'qerty', '123258', 2, 4, 3),
('kolya', 'vbnm', '0000', 2, 5, 3),
('alex', 'zxcvb', '9999', 2, 6, 4);