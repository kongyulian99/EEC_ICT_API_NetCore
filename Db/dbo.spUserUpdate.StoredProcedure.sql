USE [TestDbHungND]
GO
/****** Object:  StoredProcedure [dbo].[spUserUpdate]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hungnd
-- Create date: 02/05/2024
-- Description:	Insert, Update User
-- =============================================
CREATE PROCEDURE [dbo].[spUserUpdate]
    @piUserName varchar(50),	
	@piUserFullName nvarchar(50),
	@piPassword nvarchar(200),
	@piSalt nvarchar(50),
	@piForceLogoutKey nvarchar(50),
	@piStatus tinyint,

	@poReturnCode	INT	 = 1				OUT,		-- Code loi theo number (1: thanh cong, #1 that bai)
    @poReturnMessage NVARCHAR(max) = NULL	OUT			-- Thong bao loi chi tiet
AS
BEGIN TRANSACTION
	SET NOCOUNT ON;
	Declare 
		@v_count int
	BEGIN TRY
		select @v_count = count(1) from tUser where cUserName = @piUserName;
		IF(@v_count = 0)
		BEGIN	
			 INSERT INTO [dbo].[tUser]
				   (cUserName
				   ,cUserFullName
				   ,cPassword
				   ,cSalt
				   ,cForceLogoutKey
				   ,cStatus)
			 VALUES
				   (
				   @piUserName
				   ,@piUserFullName
				   ,@piPassword
				   ,@piSalt
				   ,@piForceLogoutKey
				   ,@piStatus);
		END
		ELSE
		BEGIN
			UPDATE [dbo].[tUser]
			   SET 
				  cUserFullName = @piUserFullName
				  ,cPassword = @piPassword
				  ,cSalt = @piSalt
				  ,cForceLogoutKey = @piForceLogoutKey
				  ,cStatus = @piStatus
			 WHERE cUserName = @piUserName;
		END
		 
		SET @poReturnCode		= 1;		
		SET @poReturnMessage	= NULL;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH	
		ROLLBACK TRANSACTION;
		SET @poReturnCode = -1;
		SET @poReturnMessage	= ERROR_MESSAGE();		
	END CATCH;
GO
