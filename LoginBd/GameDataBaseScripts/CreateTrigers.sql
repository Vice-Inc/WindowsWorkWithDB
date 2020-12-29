USE Game
GO

CREATE TRIGGER PlayersAndChests_INSERT
ON PlayersAndChests
INSTEAD OF INSERT
AS
IF (SELECT CountOfGold FROM Player WHERE Id=(SELECT IdOfPlayer FROM inserted)) >= 
	(SELECT Cost FROM Chest WHERE Id=(SELECT IdOfChest FROM inserted))
BEGIN

UPDATE Player
SET CountOfGold = CountOfGold - (SELECT Cost FROM Chest WHERE Id=(SELECT IdOfChest FROM inserted))
WHERE Id=(SELECT IdOfPlayer FROM inserted)

INSERT INTO [dbo].[PlayersAndChests] 
			(IdOfPlayer,
			IdOfChest) 
			VALUES 
			((SELECT IdOfPlayer FROM inserted),
			(SELECT IdOfChest FROM inserted))

END
GO