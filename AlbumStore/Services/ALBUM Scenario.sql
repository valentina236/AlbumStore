-- Создание контейнера базы данных Album
create database Album
go

-- Активизация контейнера базы данных Album
use Album
go

--создание доменов
create type cod from smallint
go

create type amount from smallint
go

create type nume from varchar(50) not null
go

--создаем родительские таблицы 
create table Providerr ( provider_cod cod primary key check(provider_cod > 1000 and provider_cod <= 1050),
						 provider_name nume,
						 addres varchar(70) not null,
						 phone_fax char(12) not null,
						 rascet_scet char(20) not null)
go

create table Album_format ( cod_format cod primary key check(cod_format > 0 and cod_format < 10),
							format_name nume)
go

create table Album_genre ( cod_genre cod primary key check(cod_genre > 9 and cod_genre <= 30),
						   genre_name nume)
go

create table Album_type ( cod_type cod primary key check(cod_type > 100 and cod_type <= 120),
						  typeName nume)
go

--создаем родительско-дочерние таблицы
create table Album ( album_cod cod primary key check(album_cod > 1000 and album_cod < 4000),
					 album_name nume,
					 release_date date not null,
					 artist_name nume,
					 manufacturer_name nume,
					 cod_format cod foreign key references album_format(cod_format) check(cod_format > 0 and cod_format < 10) not null,
					 cod_genre cod foreign key references album_genre(cod_genre) check(cod_genre > 9 and cod_genre <= 30) not null,
					 cod_type cod foreign key references album_type(cod_type) check(cod_type > 100 and cod_type <= 120) not null,
					 unit_price decimal(7,2) not null,
					 photo varchar(70) not null )
go

--создаем дочерние таблицы
create table TTN ( num_doc cod check(num_doc > 0) NOT NULL,
                   date_post date not null,
				   provider_cod cod foreign key references Providerr(provider_cod) check(provider_cod > 1000 and provider_cod <= 1050) not null,
				   album_cod cod foreign key references Album (album_cod) check(album_cod > 1000 and album_cod < 4000) not null,
				   number_of_albums amount check(number_of_albums > 0) not null)
go

create table Chek ( check_number cod check(check_number > 0) not null,
                    date_of_sale date default GETDATE(),
					sale_time time default convert(smalldatetime, GETDATE()),
					album_cod cod foreign key references Album(album_cod) check(album_cod > 1000 and album_cod < 4000) not null,
					number_of_albums amount check(number_of_albums > 0) default 1 )
go

create table Jurnal_uceta ( month_name varchar(20) not null,
                            album_cod cod foreign key references Album(album_cod) check(album_cod > 1000 and album_cod < 4000) not null,
							NumbeOfDeliveredAlbums amount check(NumbeOfDeliveredAlbums >= 0) not null,
							NumberOfAlbumsSold amount check(NumberOfAlbumsSold >= 0) not null )
go

CREATE TABLE LoginTable (
    IdUser SMALLINT PRIMARY KEY,
    UserName VARCHAR(30) NOT NULL,
    UserPassword VARCHAR(60) NOT NULL,
    RoleName VARCHAR(6) NOT NULL CHECK(RoleName = 'admin' OR RoleName = 'user' OR RoleName = 'seller'))
GO

--внесение информации--
insert into Providerr(provider_cod,provider_name,addres,phone_fax,rascet_scet)
values ( 1001,'KTOWN','Kannam 15','+37356912011','400981O9480200078012'),
	   ( 1002,'NTERPARK','Myeongdong 46/12','+37315947230','70100768930190354768'),
	   ( 1003, 'WEVERSE','Intaewon 41','+37321305698','19203548705040546685'),
	   ( 1004,'TARGET','Namsang 11','+37369250812','30002708459319901223'),
	   ( 1005, 'KPOP Merch','Dongdaemun 45','+37339801264','3862718456319001220'),
	   ( 1006, 'FATIRK','Lesova 19','+37329867731','60010228590004728197'),
	   ( 1007,'Gmarket','Bucuresti 47','+37316062308','56789194351000490330'),
	   ( 1008, 'COKODIVE','Sinchon 34/2','+37309554239','90000846127094546278'),
	   ( 1009,'Catchopcd','Konkuk 23','+37345219871','19628354607870090410'),
	   ( 1010,'Amazon','Linkoln Road 13/7','+37369250812','10986789012387492910')
go

insert into Album_format(cod_format,format_name)
values ( 1, 'физический'),
	   ( 2, 'Kit'),
	   ( 3, 'винил'),
	   ( 4, 'цифровой'),
	   ( 5, 'стриминговый')
go

insert into Album_genre(cod_genre,genre_name)
values ( 10, 'Кантри'),
	   ( 11, 'Латиноамериканский'),
	   ( 12, 'Блюз'),
	   ( 13, 'Джаз'),
	   ( 14, 'Шансон'),
	   ( 15, 'Электронная музыка'),
	   ( 16, 'Рок'),
	   ( 17, 'Хип-хоп'),
	   ( 18, 'Диско'),
	   ( 19, 'Поп-музыка'),
	   ( 20, 'Фанк'),
	   ( 21, 'Соул')
go

insert into Album_type(cod_type,typeName)
values ( 101, 'Студийный'),
	   ( 102, 'Концертный'),
	   ( 103, 'Дебютный'),
	   ( 104, 'Микстейп'),
	   ( 105, 'Сольный'),
	   ( 106, 'Сборник'),
	   ( 107, 'Саундтрек'),
	   ( 108, 'Трибьют-альбом'),
	   ( 109, 'Демо')
go

insert into Album(album_cod, album_name, release_date, artist_name, manufacturer_name, cod_format, cod_genre, cod_type, unit_price, photo)
values ( 1901, 'Born Pink','2022-09-16','BLACKPINK','Yg Entertainment',1,19,101,1100.5,'born_pink.jpg'),
	   ( 1902, 'Proof', '2022-06-15','BTS','Hybe Corporation',1,19,106,1550,'proof.jpeg'),
	   ( 1501, 'Chromatica','2020-05-29','Lady Gaga','Interscope Records',3,15,101,890.4,'chromatica.jpg'),
	   ( 1601, 'Teatro d’ira: Vol. I','2021-03-19','Måneskin','Sony Music Entertainment',4,16,101,780.8,'Teatro_dira_Vol1.jpg'),
	   ( 1701, 'Hope World','2018-03-02','J-Hope','Hybe Corporation',5,17,104,980,'Hope_World.png'),
	   ( 1903, 'Blackpink 2021 The Show Live','2021-06-01','BLACKPINK','Yg Entertainment',2,19,102,1000.5,'Blackpink_2021_The_Show_Live.jpg'),
	   ( 1702, 'D-2','2020-05-28','Agust D','Hybe Corporation',5,17,104,780.8,'D-2.jpg'),
	   ( 1904, 'BTS World: Original Soundtrack','2019-06-28','BTS','Hybe Corporation',4,19,107,670.30,'BTS_World_Original_Soundtrack.png'),
	   ( 1703, 'I Am Not','2018-03-26','Stray Kids','JYP Entertainment',1,17,103,679,'i_am_not.jpg'),
	   ( 1201, 'Invincible','2001-10-30','Michael Jackson','Sony Music Entertainment',3,12,101,949.9,'invincible.jpg'),
	   ( 1905, 'The Album','2020-10-02','BLACKPINK','Yg Entertainment',1,19,101, 1050.25,'the_album.jpg')
go

insert into TTN(num_doc,date_post,provider_cod,album_cod,number_of_albums)
values ( 3001,'2025-01-25',1003,1902,20),
	   ( 3002,'2025-01-12',1004,1501,15),
	   ( 3003,'2025-01-12',1001,1901,17),
	   ( 3004,'2025-02-03',1008,1601,10),
	   ( 3005,'2025-02-01',1010,1201,18),
	   ( 3006,'2025-02-07',1002,1703,16),
	   ( 3007,'2025-01-13',1003,1903,15),
	   ( 3008,'2025-02-24',1001,1702,8),
	   ( 3009,'2025-02-13',1009,1701,12),
	   ( 3010,'2025-01-31',1002,1904,13)
go

insert into Chek(check_number,date_of_sale,sale_time,album_cod,number_of_albums)
values ( 3101,'2025-02-28','15:16',1201,1),
	   ( 3102,'2025-02-26','17:18',1501,2),
	   ( 3103,'2025-01-25','19:25',1901,3),
	   ( 3104,'2025-01-18','14:05',1901,3),
	   ( 3105,'2025-02-28','12:19',1703,2),
	   ( 3106,'2025-02-28','16:34',1903,1),
	   ( 3107,'2025-01-28','11:48',1902,2),
	   ( 3108,'2025-02-26','20:23',1701,1),
	   ( 3109,'2025-01-30','17:19',1903,3),
	   ( 3110,'2025-02-01','16:55',1904,1),
	   ( 3111,'2025-02-01','11:10',1901,1),
	   ( 3112,'2025-02-25','15:40',1702,2),
	   ( 3113,'2025-03-05','10:10',1201,1),
	   ( 3114,'2025-03-07','14:45',1905,1),
	   ( 3115,'2025-03-10','16:30',1904,1),
	   ( 3116,'2025-03-12','13:08',1702,1),
	   ( 3117,'2025-03-15','17:21',1905,1)
go


insert into Jurnal_uceta(month_name,album_cod,NumbeOfDeliveredAlbums,NumberOfAlbumsSold)
values ( 'January',1901,17,10),
	   ( 'January',1903,15,10),
	   ( 'January',1501,15,7),
	   ( 'January',1904,13,0),
	   ( 'January',1902,20,2),
	   ( 'February',1601,10,4),
	   ( 'February',1703,16,9),
	   ( 'February',1201,18,11),
	   ( 'February',1701,12,10),
	   ( 'February',1702,8,5)
go

INSERT INTO LoginTable(IdUser,UserName, UserPassword, RoleName)
VALUES ( 1,'admin','$2a$12$8YNAYBrBS.y29JSwG25RKuj0tVDiIwyc0WhzQlfFxxcQIClYsSUby','admin'),
	   ( 2,'valentina','$2a$12$QmHEq.y7ScVJf5zMIjg29.YcgqW6qRRG5ClBFk0Om4Quxp7N.nW6i','user'),
	   ( 3,'user421','$2a$12$f7BBCXkPedwWV1N/OIAGB.f/bUu9v9GTuDfnR6.3Eq.e/eRSS6DNK','user'),
	   ( 4,'sel2314','$2a$12$F1ZAoA7e9KUQGTiUhy9lFOkzSw.l1Ie4.H3ohPfS.V69RetZkbmxi','seller'),
	   ( 5,'sel9607','$2a$12$0qOwNbt0Um2gZ3YzForEx.832Q9N9PpUPAesL/fCx5w0dK4VYJTum','seller'),
	   ( 6,'sel4169','$2a$12$cJEI8mPR14w0ThIFb7OtLOTeoe9WsDHHz.ZNNFBCkR1pUajUUF9Ye','seller'),
	   ( 7,'sel3202','$2a$12$b57ZpU9lKCSZ1vFL3RnAK.Vq77PIahJdqKT6yh6YgX0EOGQbHkZu6','seller')
GO

--создание индексов--
create index indx_album on Album(album_name)
go

create index indx_artist on Album(artist_name)
go

create index indx_format on Album_format(format_name)
go

create index indx_genre on Album_genre(genre_name)
go

create index indx_type on Album_type(typeName)
go

create index indx_providerName on Providerr(provider_name)
go

GO
/*создаем представление таблиц Album и Album_format, которая поможет вывести полную информацию об альбоме */
CREATE VIEW ViewAlbumFormatInfo AS
SELECT album_cod, album_name, release_date, artist_name, manufacturer_name, format_name, cod_genre, cod_type, unit_price, photo
FROM Album_format INNER JOIN Album ON Album_format.cod_format = Album.cod_format
GO

SELECT *
FROM ViewAlbumFormatInfo

GO
/*создаем представление таблицы Album_genre и представления ViewAlbumFormatInfo, которая поможет вывести полную информацию об альбоме */
CREATE VIEW ViewAlbumGenreInfo AS
SELECT album_cod, album_name, release_date, artist_name, manufacturer_name, format_name, genre_name, cod_type, unit_price, photo
FROM Album_genre INNER JOIN ViewAlbumFormatInfo ON Album_genre.cod_genre = ViewAlbumFormatInfo.cod_genre
GO

SELECT *
FROM ViewAlbumGenreInfo

GO
/*создаем представление таблицы Album_type и представления ViewAlbumGenreInfo для того, чтобы вывести полную информацию об альбоме.
  Также для быстрого и эффективного поиска информации в запросах, в которых используются данные из этих таблиц*/
CREATE VIEW ViewAlbumInfo AS
SELECT album_cod, album_name, release_date, artist_name, manufacturer_name, format_name, genre_name, typeName, unit_price, photo
FROM Album_type INNER JOIN ViewAlbumGenreInfo ON Album_type.cod_type = ViewAlbumGenreInfo.cod_type
GO

SELECT *
FROM ViewAlbumInfo

GO
/*создаем представление представления ViewAlbumInfo и таблицы Chek для того, чтобы иметь полную информацию о покупке и  
  о альбомах, которые купили. Также для быстрого и эффективного поиска информации
  в запросах, в которых используются данные из этих таблиц*/
CREATE VIEW ViewAlbumChek AS
SELECT check_number, date_of_sale, sale_time, album_name, artist_name, unit_price, number_of_albums, unit_price * number_of_albums AS TotalCount
FROM ViewAlbumInfo INNER JOIN Chek ON ViewAlbumInfo.album_cod = Chek.album_cod
GO

SELECT *
FROM ViewAlbumChek

GO
/*создаем представление представления ViewAlbumInfo и таблицы Jurnal_uceta для того, чтобы иметь информацию о альбомах и их продаж.
  Также для быстрого и эффективного поиска информации в запросах, в которых используются данные из этих таблиц*/
CREATE VIEW ViewAlbumJurnal AS
SELECT month_name, album_name, artist_name, NumbeOfDeliveredAlbums, NumberOfAlbumsSold, NumbeOfDeliveredAlbums - NumberOfAlbumsSold AS remainder
FROM ViewAlbumInfo INNER JOIN Jurnal_uceta ON ViewAlbumInfo.album_cod = Jurnal_uceta.album_cod
GO

SELECT *
FROM ViewAlbumJurnal

GO
/*создаем вспомогательное представление таблиц Providerr и TTN для того, чтобы соединить в одну таблицу отношения Providerr и Album */
CREATE VIEW ViewProviderTTN AS
SELECT num_doc, date_post, provider_name, album_cod, number_of_albums
FROM Providerr INNER JOIN TTN ON Providerr.provider_cod = TTN.provider_cod
GO

SELECT *
FROM  ViewProviderTTN

GO
/*создаем представление представлений ViewAlbumInfo и ViewProviderTTN для того, чтобы иметь полную информацию о поставках и  
  о альбомах, которые поставили. Также для быстрого и эффективного поиска информации
  в запросах, в которых используются данные из этих таблиц*/
CREATE VIEW ViewAlbumTTN AS
SELECT num_doc, date_post, provider_name, album_name, artist_name, unit_price, number_of_albums, unit_price * number_of_albums AS TotalCount
FROM ViewAlbumInfo INNER JOIN ViewProviderTTN ON ViewAlbumInfo.album_cod = ViewProviderTTN.album_cod
GO

SELECT *
FROM  ViewAlbumTTN


--функции генерации первичных ключей
GO
CREATE FUNCTION PrimIdProvider()
RETURNS SMALLINT AS
BEGIN
   DECLARE @LNIdProvider SMALLINT
   SET @LNIdProvider = (SELECT MAX(provider_cod) + 1 FROM Providerr)
   RETURN @LNIdProvider
END
GO

GO
CREATE FUNCTION PrimIdAlbum(@cod_genre SMALLINT)
RETURNS SMALLINT AS
BEGIN
   DECLARE @LNIdAlbum SMALLINT,
		   @LNcountA SMALLINT
   SET @LNcountA = ( SELECT COUNT(album_cod) FROM Album
					 WHERE cod_genre = @cod_genre )
   SET @LNIdAlbum = @cod_genre * 100 + @LNcountA + 1
   RETURN @LNIdAlbum
END
GO

GO
CREATE FUNCTION PrimIdUser()
RETURNS SMALLINT AS
BEGIN
   DECLARE @LNIdUser SMALLINT
   SET @LNIdUser = (SELECT MAX(IdUser) + 1 FROM LoginTable)
   RETURN @LNIdUser
END
GO

GO
CREATE FUNCTION GenerateTTNNumDoc()
RETURNS SMALLINT AS
BEGIN
   DECLARE @LNNumDoc SMALLINT
   SET @LNNumDoc = (SELECT MAX(num_doc) + 1 FROM TTN)
   RETURN @LNNumDoc
END
GO

GO
CREATE FUNCTION GenerateCheckNum()
RETURNS SMALLINT AS
BEGIN
   DECLARE @LNCheckNum SMALLINT
   SET @LNCheckNum = (SELECT MAX(check_number) + 1 FROM Chek)
   RETURN @LNCheckNum
END
GO

--процедуры

--процедуры для добавления информации
GO
CREATE PROCEDURE AddAlbum
    @album_name varchar(50),
    @release_date DATE,
    @artist_name varchar(50),
    @manufacturer_name varchar(50),
    @cod_format SMALLINT,
    @cod_genre SMALLINT,
    @cod_type SMALLINT,
    @unit_price DECIMAL(7, 2), 
	@photo VARCHAR(70)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO Album OUTPUT INSERTED.album_cod VALUES (dbo.PrimIdAlbum(@cod_genre), @album_name, @release_date, @artist_name, @manufacturer_name, @cod_format, @cod_genre, @cod_type, @unit_price, @photo)
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить новую запись в таблицу.', 1;
	END CATCH
END
GO

GO
CREATE PROCEDURE AddProvider
    @provider_name VARCHAR(50),
    @addres VARCHAR(70),
    @phone_fax CHAR(12),
    @rascet_scet CHAR(20)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO Providerr OUTPUT INSERTED.provider_cod VALUES (dbo.PrimIdProvider(),@provider_name,@addres,@phone_fax,@rascet_scet)
			COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить новую запись в таблицу.', 1;
	END CATCH
END
GO

GO
CREATE PROCEDURE AddTTN
    @date_post DATE,
    @provider_cod SMALLINT,
    @album_cod SMALLINT,
    @number_of_albums SMALLINT
AS
BEGIN
  BEGIN TRY
    BEGIN TRANSACTION
      INSERT INTO TTN VALUES (dbo.GenerateTTNNumDoc(), @date_post, @provider_cod, @album_cod, @number_of_albums)
      COMMIT TRANSACTION
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить новую запись в таблицу.', 1;
  END CATCH
END
GO

GO
CREATE PROCEDURE AddCheck
    @album_cod SMALLINT,
    @number_of_albums SMALLINT
AS
BEGIN
  BEGIN TRY
    BEGIN TRANSACTION
      INSERT INTO Chek(check_number, album_cod, number_of_albums) 
	  VALUES (dbo.GenerateCheckNum(), @album_cod, @number_of_albums)
      COMMIT TRANSACTION
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить новую запись в таблицу.', 1;
  END CATCH
END
GO

GO
CREATE PROCEDURE AddJurnalInfo
    @month_name VARCHAR(20),
    @album_cod SMALLINT,
    @NumbeOfDeliveredAlbums SMALLINT,
    @NumberOfAlbumsSold SMALLINT
AS
BEGIN
  BEGIN TRY
    BEGIN TRANSACTION
      INSERT INTO Jurnal_uceta(month_name, album_cod, NumbeOfDeliveredAlbums, NumberOfAlbumsSold) 
	  VALUES (@month_name, @album_cod, @NumbeOfDeliveredAlbums, @NumberOfAlbumsSold)
      COMMIT TRANSACTION
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить новую запись в таблицу.', 1;
  END CATCH
END
GO

GO
CREATE PROCEDURE AddUser
    @UserName VARCHAR(30),
    @UserPassword VARCHAR(60),
    @RoleName VARCHAR(6)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO LoginTable OUTPUT INSERTED.IdUser VALUES (dbo.PrimIdUser(),@UserName,@UserPassword,@RoleName)
			COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось добавить нового пользователя.', 1;
	END CATCH
END
GO

--процедуры для изменения информации
GO
CREATE PROCEDURE UpdateAlbum
    @album_cod SMALLINT,
    @album_name varchar(50),
    @release_date DATE,
    @artist_name varchar(50),
    @manufacturer_name varchar(50),
    @cod_format SMALLINT,
    @cod_genre SMALLINT,
    @cod_type SMALLINT,
    @unit_price DECIMAL(7, 2),
	@photo VARCHAR(70)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Album
        SET 
            album_name = @album_name,
            release_date = @release_date,
            artist_name = @artist_name,
            manufacturer_name = @manufacturer_name,
            cod_format = @cod_format,
            cod_genre = @cod_genre,
            cod_type = @cod_type,
            unit_price = @unit_price,
			photo = @photo
        WHERE
            album_cod = @album_cod

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось изменить информацию.', 1;
    END CATCH
END;

GO
CREATE PROCEDURE UpdateProvider
    @provider_cod SMALLINT,
    @provider_name VARCHAR(50),
    @addres VARCHAR(70),
    @phone_fax CHAR(12),
    @rascet_scet CHAR(20)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Providerr
        SET 
            provider_name = @provider_name,
            addres = @addres,
            phone_fax = @phone_fax,
            rascet_scet = @rascet_scet
        WHERE
            provider_cod = @provider_cod

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось изменить информацию.', 1;
    END CATCH
END

GO
CREATE PROCEDURE UpdateTTN
    @num_doc SMALLINT,
    @provider_cod SMALLINT,
    @album_cod SMALLINT,
    @number_of_albums SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TTN
        SET 
            provider_cod = @provider_cod,
            album_cod = @album_cod,
            number_of_albums = @number_of_albums
        WHERE
            num_doc = @num_doc

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
		THROW 50000, 'Не удалось изменить информацию.', 1;
    END CATCH
END

GO
CREATE PROCEDURE UpdateChek
    @check_number SMALLINT,
    @album_cod SMALLINT,
    @number_of_albums SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Chek
        SET 
            album_cod = @album_cod,
            number_of_albums = @number_of_albums
        WHERE
            check_number = @check_number

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
		THROW 50000, 'Не удалось изменить информацию.', 1;
    END CATCH
END

GO
CREATE PROCEDURE UpdateJurnalUceta
    @month_name VARCHAR(20),
    @album_cod SMALLINT,
    @NumbeOfDeliveredAlbums SMALLINT,
    @NumberOfAlbumsSold SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Jurnal_uceta
        SET 
            NumbeOfDeliveredAlbums = @NumbeOfDeliveredAlbums,
            NumberOfAlbumsSold = @NumberOfAlbumsSold
        WHERE
            month_name = @month_name
            AND album_cod = @album_cod;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
		THROW 50000, 'Не удалось изменить информацию.', 1;
    END CATCH
END

GO
CREATE PROCEDURE UpdateUserInfo
    @IdUser SMALLINT,
    @UserName VARCHAR(30),
    @UserPassword VARCHAR(60),
    @RoleName VARCHAR(6)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE LoginTable
        SET 
            UserName = @UserName,
            UserPassword = @UserPassword,
            RoleName = @RoleName
        WHERE
            IdUser = @IdUser

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось изменить информацию пользователя.', 1;
    END CATCH
END

--процедуры для удаления информации
GO
CREATE PROCEDURE DeleteRecordsProvider
    @recordID SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Providerr WHERE provider_cod = @recordID)
		BEGIN
            DELETE FROM Providerr WHERE provider_cod = @recordID
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Запись не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Нельзя удалить запись, так как она связана с другими таблицами.', 1;
    END CATCH;
END;

GO
CREATE PROCEDURE DeleteRecordsAlbum
    @recordID SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Album WHERE album_cod = @recordID)
		BEGIN
            DELETE FROM Album WHERE album_cod = @recordID
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Запись не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Нельзя удалить запись, так как она связана с другими таблицами.', 1;
    END CATCH;
END;

go
CREATE PROCEDURE DeleteRecordsTTN
    @recordID SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM TTN WHERE num_doc = @recordID)
		BEGIN
            DELETE FROM TTN WHERE num_doc = @recordID
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Запись не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Нельзя удалить запись, так как она связана с другими таблицами.', 1;
    END CATCH;
END;

go
CREATE PROCEDURE DeleteRecordsCheck
    @recordID SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Chek WHERE check_number = @recordID)
		BEGIN
            DELETE FROM Chek WHERE check_number = @recordID
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Запись не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Нельзя удалить запись, так как она связана с другими таблицами.', 1;
    END CATCH;
END;

GO
CREATE PROCEDURE DeleteRecordsUcet
    @month varchar(20),
    @name varchar(50)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Jurnal_uceta WHERE month_name = @month AND album_cod IN (SELECT album_cod FROM Album WHERE album_name = @name))
		BEGIN
            DELETE FROM Jurnal_uceta WHERE month_name = @month AND album_cod IN (SELECT album_cod FROM Album WHERE album_name = @name);
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Запись не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Нельзя удалить запись, так как она связана с другими таблицами.', 1;
    END CATCH;
END;

GO
CREATE PROCEDURE DeleteUser
    @UserID SMALLINT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM LoginTable WHERE IdUser = @UserID)
		BEGIN
            DELETE FROM LoginTable WHERE IdUser = @UserID
            COMMIT TRANSACTION;
        END
        ELSE
        BEGIN;
            THROW 50000, 'Такого пользователя не существует в базе данных', 1;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW 50000, 'Не удалось удалить пользователя.', 1;
    END CATCH;
END;

--триггеры
--триггер, который обновляет кол-во проданных альбомов в таблице Jurnal_uceta после добавления новой записи в таблицу Chek
GO
CREATE TRIGGER UpdateSales
ON Chek
AFTER INSERT
AS
BEGIN
    DECLARE @month_name varchar(9)
    DECLARE @sold_albums smallint
    DECLARE @cod_albums smallint

    SELECT @cod_albums = album_cod, @sold_albums = number_of_albums, @month_name = DATENAME(month, date_of_sale) 
  FROM inserted

    IF EXISTS (SELECT 1
               FROM Jurnal_uceta
               WHERE album_cod = @cod_albums AND month_name = @month_name AND (NumbeOfDeliveredAlbums <= @sold_albums OR NumbeOfDeliveredAlbums = NumberOfAlbumsSold OR @sold_albums + NumberOfAlbumsSold > NumbeOfDeliveredAlbums))
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51000, 'Произошла ошибка! Не удается купить столько альбомов', 1;
    END
    ELSE
    BEGIN
        UPDATE Jurnal_uceta
        SET NumberOfAlbumsSold = NumberOfAlbumsSold + @sold_albums
        WHERE album_cod = @cod_albums AND month_name = @month_name
    END
END
GO

--триггер, который обновляет кол-во поставленных альбомов в таблице Jurnal_uceta после добавления новой записи в таблицу TTN
GO
CREATE TRIGGER UpdateDeliveries
ON TTN
AFTER INSERT
AS
BEGIN
    DECLARE @month_name varchar(9)
    DECLARE @delivered_albums smallint
    DECLARE @cod_albums smallint

    SELECT @cod_albums = album_cod, @delivered_albums = number_of_albums, @month_name = DATENAME(month, date_post) 
	FROM inserted

    UPDATE Jurnal_uceta
    SET NumbeOfDeliveredAlbums = NumbeOfDeliveredAlbums + @delivered_albums
    WHERE album_cod = @cod_albums AND month_name = @month_name
END
GO

--триггер, который обновляет кол-во проданных альбомов в таблице Jurnal_uceta после удаления записи в таблице Chek
GO
CREATE TRIGGER DecreaseSales
ON Chek
AFTER DELETE
AS
BEGIN
    DECLARE @month_name varchar(9)
    DECLARE @sold_albums smallint
    DECLARE @cod_albums smallint

    SELECT @cod_albums = album_cod, @sold_albums = number_of_albums, @month_name = DATENAME(month, date_of_sale) 
	FROM deleted

    UPDATE Jurnal_uceta
    SET NumberOfAlbumsSold = NumberOfAlbumsSold - @sold_albums
    WHERE album_cod = @cod_albums AND month_name = @month_name;
END
GO