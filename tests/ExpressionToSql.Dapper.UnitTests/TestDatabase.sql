create database TestDatabase
go

create table TestDatabase.dbo.Person(Id int not null primary key identity(1,1), Name varchar(50) null)
go