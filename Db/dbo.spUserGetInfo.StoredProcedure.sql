USE [TestDbHungND]
GO
/****** Object:  StoredProcedure [dbo].[spUserGetInfo]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spUserGetInfo]
	@piUserName varchar(50)
AS
BEGIN
	select * from tUser where cUserName = @piUserName;
END

GO
