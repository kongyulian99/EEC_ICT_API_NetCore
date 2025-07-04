USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tGroupRolePermission]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tGroupRolePermission](
	[ID] [uniqueidentifier] NOT NULL,
	[fkGroupRoleId] [nvarchar](50) NULL,
	[fkPermissionId] [nvarchar](50) NULL,
 CONSTRAINT [PK_tGroupRolePermission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tGroupRolePermission] ADD  CONSTRAINT [DF_tGroupRolePermission_ID]  DEFAULT (newid()) FOR [ID]
GO
