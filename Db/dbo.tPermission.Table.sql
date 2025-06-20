USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tPermission]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tPermission](
	[ID] [uniqueidentifier] NOT NULL,
	[cPermissionCode] [varchar](50) NULL,
	[cPermissionName] [nvarchar](250) NULL,
	[cStatus] [tinyint] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tPermission] ADD  CONSTRAINT [DF_tPermission_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[tPermission] ADD  CONSTRAINT [DF_tPermission_cStatus]  DEFAULT ((1)) FOR [cStatus]
GO
