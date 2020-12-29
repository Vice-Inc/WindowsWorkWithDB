USE [Game]
GO

INSERT INTO [dbo].[Player]
           ([LoginEmail]
           ,[Password]
           ,[Nickname]
           ,[DateOfRegistration]
           ,[DateOfLastOnline]
           ,[CountOfGold]
           ,[Level]
           ,[Role])
     VALUES
           ('fox@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'Fox'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'10'
           ,'0'),


		   ('admin@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'Admin'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'90'
           ,'1'),

		   --('k-d-a-200@mail.ru'
     --      ,'gnzLDuqKcGxMNKFokfhOew=='
     --      ,'ViceInc'
     --      ,'2007-01-01'
     --      ,'2020-09-09'
     --      ,'1000'
     --      ,'404'
     --      ,'1'),

		   ('cat@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'Cat'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'14'
           ,'1'),

		   ('gg@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'GG'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'23'
           ,'1'),

		   ('dog@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'Dog'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'16'
           ,'1'),

		   ('man@mail.ru'
           ,'gnzLDuqKcGxMNKFokfhOew=='
           ,'Man'
           ,'2007-01-01'
           ,'2020-09-09'
           ,'1000'
           ,'65'
           ,'1')
GO




INSERT INTO [dbo].[Items]
			([Name]
			,[Cost]
			,[Damage]
			,[AttackBonus]
			,[PtotectionBonus]
			,[MagicBonus]
			,[Rarity])
			VALUES
			('�����'
			,'10'
			,'1'
			,'0'
			,'0'
			,'0'
			,'����������'),

			('������'
			,'15'
			,'1'
			,'0'
			,'0'
			,'0'
			,'����������'),

			--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

			('���'
			,'100'
			,'2'
			,'0'
			,'0'
			,'0'
			,'�������'),

			('���'
			,'120'
			,'0'
			,'3'
			,'15'
			,'0'
			,'�������'),

			('�����'
			,'130'
			,'3'
			,'1'
			,'1'
			,'0'
			,'�������'),

			--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

			('�����'
			,'150'
			,'4'
			,'1'
			,'1'
			,'0'
			,'������'),

			('���'
			,'200'
			,'8'
			,'1'
			,'2'
			,'0'
			,'������'),

			('����� ����'
			,'230'
			,'0'
			,'0'
			,'0'
			,'10'
			,'������'),

			--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

			('���'
			,'400'
			,'10'
			,'0'
			,'0'
			,'0'
			,'����� ������'),

			('������� ���'
			,'490'
			,'18'
			,'5'
			,'4'
			,'0'
			,'����� ������'),

			('������ ����'
			,'520'
			,'0'
			,'0'
			,'0'
			,'25'
			,'����� ������'),

			--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

			('�����'
			,'1000'
			,'0'
			,'0'
			,'0'
			,'50'
			,'����������'),

			('�������� ����'
			,'5000'
			,'0'
			,'25'
			,'25'
			,'25'
			,'����������')
GO



INSERT INTO [dbo].[Chest]
			([Name]
			,[Cost]
			,[Bauble]
			,[Usual]
			,[Rare]
			,[SuperRare]
			,[Unique])
			VALUES
			('����������'
			,'12'
			,'1'
			,'0'
			,'0'
			,'0'
			,'0'),

			('�������'
			,'110'
			,'0'
			,'1'
			,'0'
			,'0'
			,'0'),

			('������'
			,'180'
			,'0'
			,'0'
			,'1'
			,'0'
			,'0'),

			('����� ������'
			,'450'
			,'0'
			,'0'
			,'0'
			,'1'
			,'0'),

			('����������'
			,'2500'
			,'0'
			,'0'
			,'0'
			,'0'
			,'1'),

			('���������'
			,'500'
			,'8'
			,'6'
			,'4'
			,'2'
			,'0'),

			('�������'
			,'750'
			,'10'
			,'8'
			,'6'
			,'4'
			,'2'),

			('�������'
			,'1000'
			,'5'
			,'4'
			,'3'
			,'2'
			,'1')
GO