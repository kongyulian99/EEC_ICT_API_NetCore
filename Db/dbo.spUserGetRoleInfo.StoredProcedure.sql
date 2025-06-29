USE [TestDbHungND]
GO
/****** Object:  StoredProcedure [dbo].[spUserGetRoleInfo]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[spUserGetRoleInfo]
    @piUserName	VARCHAR(50)	
AS
BEGIN
	DECLARE @JSON nvarchar(max);
	SET @JSON = 
	(
		select 
			distinct
			T1.ID
			,T1.cUserName
			,T1.cUserFullName
			,T1.cStatus
				,roles.ID as fkRoleId
				,roles.cRoleCode
				,roles.cRoleName
					,[permissions].ID as fkPermissionId
					,[permissions].cPermissionCode
					,[permissions].cPermissionName
			from tUser T1
			left join tUserGroup T2 on T1.ID = T2.fkUserId
			left join tGroup T3 on T2.fkGroupId = T3.ID
			left join tGroupRole T4 on T2.fkGroupId = T4.fkGroupId
			left join tRole roles on T4.fkRoleId = roles.ID
			left join tGroupRolePermission T6 on T4.ID = t6.fkGroupRoleId
			left join tPermission [permissions] on T6.fkPermissionId = [permissions].ID
			where 
			T1.cUserName = @piUserName and t1.cStatus = 1	 	
			group by
			T1.ID
			,T1.cUserName
			,T1.cUserFullName
			,T1.cStatus
				,roles.ID
				,roles.cRoleCode
				,roles.cRoleName
					,[permissions].ID
					,[permissions].cPermissionCode
					,[permissions].cPermissionName
			order by 
			 T1.ID
			,T1.cUserName
			,T1.cUserFullName
			,T1.cStatus
				,roles.ID
				,roles.cRoleCode
				,roles.cRoleName
					,[permissions].ID
					,[permissions].cPermissionCode
					,[permissions].cPermissionName	
			FOR JSON AUTO
	);
	select @JSON as 'jsonString';
END;
GO
