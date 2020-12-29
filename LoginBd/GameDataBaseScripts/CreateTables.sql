USE [Game]
GO

/****** Object:  Table [dbo].[Achievements]    Script Date: 05.09.2020 19:01:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*Создание таблицы игроков*/
CREATE TABLE [dbo].[Player](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[LoginEmail] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Nickname] [varchar](50) NOT NULL,
	[DateOfRegistration] [date] NOT NULL,
	[DateOfLastOnline] [date] NOT NULL,
	[CountOfGold] [int] NOT NULL,
	[CreditCard] [char](16) NULL,
	[Level] [int] NOT NULL,
	[Role] [bit] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы предметов*/
CREATE TABLE [dbo].[Items](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[Name] [nvarchar](50) NOT NULL,
	[Cost] [int] NOT NULL,
	[Damage] [int] NOT NULL,
	[AttackBonus] [int] NOT NULL,
	[PtotectionBonus] [int] NOT NULL,
	[MagicBonus] [int] NOT NULL,
	[Rarity] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы для соотношения игроков с их предметами*/
CREATE TABLE [dbo].[PlayersAndItems](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfPlayer] [int] NOT NULL,
	[IdOfItem] [int] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы онлайна*/
CREATE TABLE [dbo].[OnlineTable](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfPlayer] [int] NOT NULL,
	[isOnline] [bit] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы сундуков*/
CREATE TABLE [dbo].[Chest](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[Name] [nvarchar](50) NOT NULL,
	[Cost] [int] NOT NULL,
	[Bauble] [int] NOT NULL,
	[Usual] [int] NOT NULL,
	[Rare] [int] NOT NULL,
	[SuperRare] [int] NOT NULL,
	[Unique] [int] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы для соотношения игроков с их сундуками*/
CREATE TABLE [dbo].[PlayersAndChests](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfPlayer] [int] NOT NULL,
	[IdOfChest] [int] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы друзей*/
CREATE TABLE [dbo].[Friends](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfFirstFriend] [int] NOT NULL,
	[IdOfSecondFriend] [int] NOT NULL,
	[BothSide] [nvarchar](20) NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы диалогов*/
CREATE TABLE [dbo].[Dialogs](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfFirst] [int] NOT NULL,
	[IdOfSecond] [int] NOT NULL
) ON [PRIMARY]
GO

/*Создание таблицы диалогов*/
CREATE TABLE [dbo].[Message](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[IdOfDialog] [int] NOT NULL,
	[IdOfSender] [int] NOT NULL,
	[Text] [nvarchar](300) NOT NULL
) ON [PRIMARY]
GO

/*////////////////////////////////////
	Player
*/

/*Уникальность логина игрока*/
ALTER TABLE Player
ADD CONSTRAINT UQ_Player_LoginEmail UNIQUE (LoginEmail)
GO

/*Уникальность ника игрока*/
ALTER TABLE Player
ADD CONSTRAINT UQ_Player_Nickname UNIQUE (Nickname)
GO

/*Значение по умолчанию для DateOfRegistration*/
ALTER TABLE Player
ADD CONSTRAINT DF_Player_DateOfRegistration DEFAULT (getdate()) FOR DateOfRegistration
GO

/*Значение по умолчанию для DateOfLastOnline*/
ALTER TABLE Player
ADD CONSTRAINT DF_Player_DateOfLastOnline DEFAULT (getdate()) FOR DateOfLastOnline
GO

/*////////////////////////////////////
	Items
*/

/*Уникальность названия предмета*/
ALTER TABLE Items
ADD CONSTRAINT UQ_Items_Name UNIQUE ([Name])
GO

/*Значение по умолчанию для DateOfRegistration*/
ALTER TABLE Items
ADD CONSTRAINT DF_Items_Rarity DEFAULT 'Обычный' FOR Rarity
GO

/*////////////////////////////////////
	PlayersAndItems
*/

/*Связь игрока и предметов*/
ALTER TABLE PlayersAndItems
WITH CHECK ADD CONSTRAINT FK_PlayersAndItems_Player FOREIGN KEY (IdOfPlayer)
REFERENCES Player(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*Связь игрока и предметов*/
ALTER TABLE PlayersAndItems
WITH CHECK ADD CONSTRAINT FK_PlayersAndItems_Item FOREIGN KEY (IdOfItem)
REFERENCES Items(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*////////////////////////////////////
	OnlineTable
*/

/*Связь игрока и таблицы онлайна*/
ALTER TABLE OnlineTable
WITH CHECK ADD CONSTRAINT FK_OnlineTable_Player FOREIGN KEY (IdOfPlayer)
REFERENCES Player(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*////////////////////////////////////
	Chest
*/

/*Уникальность названия сндука*/
ALTER TABLE Chest
ADD CONSTRAINT UQ_Chest_Name UNIQUE ([Name])
GO

/*////////////////////////////////////
	PlayersAndChests
*/

/*Связь игрока и сундука*/
ALTER TABLE PlayersAndChests
WITH CHECK ADD CONSTRAINT FK_PlayersAndChests_Player FOREIGN KEY (IdOfPlayer)
REFERENCES Player(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*Связь игрока и сундука*/
ALTER TABLE PlayersAndChests
WITH CHECK ADD CONSTRAINT FK_PlayersAndChests_Chest FOREIGN KEY (IdOfChest)
REFERENCES Chest(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*////////////////////////////////////
	Friends
*/

/*Связь игрока и друга*/
ALTER TABLE Friends
WITH CHECK ADD CONSTRAINT FK_Friends_IdOfFirstFriend FOREIGN KEY (IdOfFirstFriend)
REFERENCES Player(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*////////////////////////////////////
	Dialogs
*/

/*Связь игрока и диалога*/
ALTER TABLE Dialogs
WITH CHECK ADD CONSTRAINT FK_Dialogs_IdOfFirst FOREIGN KEY (IdOfFirst)
REFERENCES Player(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO

/*////////////////////////////////////
	Message
*/

/*Связь игрока и диалога*/
ALTER TABLE [Message]
WITH CHECK ADD CONSTRAINT FK_Message_IdOfDialog FOREIGN KEY (IdOfDialog)
REFERENCES Dialogs(Id)
ON UPDATE CASCADE
ON DELETE CASCADE
GO