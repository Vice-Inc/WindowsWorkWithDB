Use Game

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetRole]
    @login varchar(50),
    @password varchar(50)
AS
    Select [Id], [Role] From [dbo].[Player] WHERE [LoginEmail]=@login AND [Password]=@password
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetDBConnectionString]
    @loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	Select [Id], [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLoginByNick]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Nickname varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [LoginEmail] FROM [dbo].[Player] WHERE [Nickname] = @_Nickname
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_CheckUser_CheckLogin]
    @login varchar(50)
AS
	Select [Role] From [dbo].[Player] WHERE LoginEmail=@login
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_CheckUser_CheckNick]
	@nick varchar(50)
AS
	Select [Role] From [dbo].[Player] WHERE Nickname=@nick
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_Registation]
    @login varchar(50),
	@password varchar(50),
	@nick varchar(50),
	@dateOfRegistration date,
	@dateOfLastOnline date,
	@creditCard char(16)

AS
    INSERT INTO [dbo].[Player] 
				(LoginEmail,
				[Password],
				Nickname,
				DateOfRegistration,
				DateOfLastOnline,
				CountOfGold,
				CreditCard,
				[Level],
				[Role])
				VALUES 
				(@login, 
				@password,
				@nick,
				@dateOfRegistration,
				@dateOfLastOnline,
				'1000',
				@creditCard,
				'1',
				'0')
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPageInfo]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @login varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)
	
	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		Select [Id], [Nickname], [Level], [CountOfGold], [DateOfRegistration], [CreditCard], [Role] From [dbo].[Player] WHERE LoginEmail=@login
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_AddFriend]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_BothSide nvarchar(20),
	@_IdOfFirstFriend int,
	@_IdOfSecondFriend int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)
	DECLARE @OldBothSide nvarchar(20) = (Select [BothSide] From [dbo].[Friends] WHERE IdOfFirstFriend=@_IdOfFirstFriend AND IdOfSecondFriend=@_IdOfSecondFriend)

	IF(@OldBothSide='Друг')
		THROW 50001, 'Invalid @OldBothSide = Друг', 1

	IF(@OldBothSide='Ожидание' AND @_BothSide!='Подарок')
		THROW 50001, 'Error @OldBothSide = Ожидание but @_BothSide!=Подарок', 1

	IF(@OldBothSide='Подписчик' AND @_BothSide!='Друг')
		THROW 50001, 'Error @OldBothSide = Подписчик but @_BothSide!=Друг', 1

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		UPDATE [dbo].[Friends] SET BothSide=@_BothSide WHERE IdOfFirstFriend=@_IdOfFirstFriend AND IdOfSecondFriend=@_IdOfSecondFriend
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_AddSearchFriend]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_BothSide nvarchar(20),
	@_IdOfFirstFriend int,
	@_IdOfSecondFriend int
AS
	IF (@_BothSide='Друг')
		THROW 50001, 'Invalid @_BothSide (Друг)', 1

	IF (@_BothSide='Подарок')
		THROW 50001, 'Invalid @_BothSide (Подарок)', 1

	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)
	DECLARE @IdOfLine int = (Select [Id] From [dbo].[Friends] WHERE IdOfFirstFriend=@_IdOfFirstFriend AND IdOfSecondFriend=@_IdOfSecondFriend)
	
	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF(@IdOfLine IS NOT NULL)
			THROW 50001, 'They are already friends', 1
		ELSE
			INSERT INTO [dbo].[Friends] 
				(IdOfFirstFriend,
				IdOfSecondFriend,
				BothSide)
				VALUES 
				(@_IdOfFirstFriend,
				@_IdOfSecondFriend,
				@_BothSide)
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetTopPlayers]
	@loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT Nickname, Level FROM [dbo].[Player] ORDER BY Level DESC
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetAllPlayers]
	@loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT Id, Nickname FROM [dbo].[Player] ORDER BY Level DESC
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetFriends]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Id int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [Friends].[Id], [Friends].[BothSide], [PlayerII].[Nickname]
		FROM [dbo].[Player] PlayerI 
		LEFT JOIN [dbo].[Friends] ON [PlayerI].[Id] = [dbo].[Friends].[IdOfFirstFriend]
		LEFT JOIN [dbo].[Player] PlayerII ON [PlayerII].[Id] = [dbo].[Friends].[IdOfSecondFriend]
		WHERE [PlayerI].[Id] = @_Id
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPresentItem]
	@loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [Id], [Name] FROM [dbo].[Items]
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPresent]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_IdOfPlayer int,
	@_IdOfItem int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		INSERT INTO [dbo].[PlayersAndItems] (IdOfPlayer, IdOfItem) VALUES (@_IdOfPlayer, @_IdOfItem)
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_IsPlayerOnline]
    @_id int
AS
    Select [isOnline] From [dbo].[OnlineTable] WHERE IdOfPlayer=@_id
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_AddPlayerOnline]
    @_IdOfPlayer int
AS
    INSERT INTO [dbo].[OnlineTable] (IdOfPlayer, isOnline) VALUES (@_IdOfPlayer, '0')
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_SetPlayerOnline]
    @_IdOfPlayer int,
	@_isOnline bit
AS
    UPDATE [dbo].[OnlineTable] SET isOnline=@_isOnline WHERE IdOfPlayer=@_IdOfPlayer
GO




-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetItems]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Id int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [dbo].[Items].[Name] FROM [dbo].[PlayersAndItems]
		LEFT JOIN [dbo].[Items] ON [dbo].[PlayersAndItems].[IdOfItem] = [dbo].[Items].[Id]
		WHERE [dbo].[PlayersAndItems].[IdOfPlayer] = @_Id
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetAllItems]
	@loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [dbo].[Items].[Name] FROM [dbo].[Items]
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetItemInfo]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [Cost], [Damage], [AttackBonus], [PtotectionBonus], [MagicBonus], [Rarity]
		FROM [dbo].[Items]
		WHERE [Name] = @_Name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetItemIdByName]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		Select [Id] From [dbo].[Items] WHERE [Name]=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_AddItemToPlayer]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_IdOfPlayer int,
	@_IdOfItem int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		INSERT INTO [dbo].[PlayersAndItems] (IdOfPlayer, IdOfItem) VALUES (@_IdOfPlayer, @_IdOfItem)
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_DeleteItemFromPlayer]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_IdOfPlayer int,
	@_IdOfItem int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		DELETE FROM [dbo].[PlayersAndItems] WHERE IdOfPlayer=@_IdOfPlayer AND IdOfItem = @_IdOfItem
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeItemInfo]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_newName nvarchar(50),
	@_name nvarchar(50),
	@_cost int,
	@_damage int,
	@_attackBonus int,
	@_ptotectionBonus int,
	@_magicBonus int,
	@_rarity nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Items] SET [Name]=@_newName, Cost=@_cost, Damage=@_damage,
			AttackBonus=@_attackBonus, PtotectionBonus=@_ptotectionBonus,
			MagicBonus=@_magicBonus, Rarity=@_rarity WHERE [Name]=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_CreateItem]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			INSERT INTO [dbo].[Items] 
				([Name],
				[Cost],
				[Damage],
				[AttackBonus],
				[PtotectionBonus],
				[MagicBonus],
				[Rarity]) 
				VALUES 
				(@_name,
				'100',
				'0',
				'0',
				'0',
				'0',
				'Обычный')
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_RemoveItem]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			DELETE FROM [dbo].[Items] WHERE Name=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetChests]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Id int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [dbo].[Chest].[Name] FROM [dbo].[PlayersAndChests]
		LEFT JOIN [dbo].[Chest] ON [dbo].[PlayersAndChests].[IdOfChest] = [dbo].[Chest].[Id]
		WHERE [dbo].[PlayersAndChests].[IdOfPlayer] = @_Id
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetAllChests]
	@loginFrom varchar(50),
    @passwordFrom varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [dbo].[Chest].[Name] FROM [dbo].[Chest]
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetChestInfo]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_Name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [Cost], [Bauble], [Usual], [Rare], [SuperRare], [Unique]
		FROM [dbo].[Chest]
		WHERE [Name] = @_Name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetChestIdByName]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		Select [Id] From [dbo].[Chest] WHERE Name=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_AddChestToPlayer]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_IdOfPlayer int,
	@_IdOfChest int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		INSERT INTO [dbo].[PlayersAndChests] (IdOfPlayer, IdOfChest) VALUES (@_IdOfPlayer, @_IdOfChest)
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_DeleteChestFromPlayer]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_IdOfPlayer int,
	@_IdOfChest int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		DELETE FROM [dbo].[PlayersAndChests] WHERE IdOfPlayer=@_IdOfPlayer AND IdOfChest = @_IdOfChest
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeChestInfo]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_newName nvarchar(50),
	@_name nvarchar(50),
	@_cost int,
	@_bauble int,
	@_usual int,
	@_rare int,
	@_superRare int,
	@_unique int
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Chest] SET Name=@_newName, Cost=@_cost, Bauble=@_bauble,
			Usual=@_usual, Rare=@_rare, SuperRare=@_superRare, [Unique]=@_unique WHERE Name=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLootItem]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_rarity nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		SELECT [Id], [Name] FROM [dbo].[Items] WHERE Rarity=@_rarity
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_CreateChest]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			INSERT INTO [dbo].[Chest] 
				([Name],
				[Cost],
				[Bauble],
				[Usual],
				[Rare],
				[SuperRare],
				[Unique])
				VALUES 
				(@_name,
				'100',
				'1',
				'1',
				'1',
				'1',
				'1')
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_RemoveChest]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_name nvarchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			DELETE FROM [dbo].[Chest] WHERE Name=@_name
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_IsErrorLogin]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @login varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		Select [Role] From [dbo].[Player] WHERE LoginEmail=@login
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeLogin]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @login varchar(50),
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET LoginEmail=@login WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_IsErrorNick]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @nick varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		Select [Role] From [dbo].[Player] WHERE Nickname=@nick
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeNick]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @nick varchar(50),
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET Nickname=@nick WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeCredit]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_newCredit char(16),
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET CreditCard=@_newCredit WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeGold]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_countOfGold int,
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET CountOfGold=@_countOfGold WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeLevel]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_level int,
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET Level=@_level WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeRole]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_role bit,
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET Role=@_role WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangePassword]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @_newPassword varchar(50),
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET Password=@_newPassword WHERE LoginEmail=@nowLogin
GO


-- =============================================
-- =============================================
CREATE PROCEDURE [dbo].[SP_ChangeDateOfLastOnline]
	@loginFrom varchar(50),
    @passwordFrom varchar(50),

    @dateOfLastOnline date,
	@nowLogin varchar(50)
AS
	DECLARE @RoleFrom bit = (Select [Role] From [dbo].[Player] WHERE [LoginEmail]=@loginFrom AND [Password]=@passwordFrom)

	IF (@RoleFrom IS NULL)
		THROW 50001, 'Invalid Sender`s login or password', 1
	ELSE
		IF (@RoleFrom=0 AND @loginFrom!=@nowLogin)
			THROW 50001, 'Sender is not admin', 1
		ELSE
			UPDATE [dbo].[Player] SET DateOfLastOnline=@dateOfLastOnline WHERE LoginEmail=@nowLogin
GO
