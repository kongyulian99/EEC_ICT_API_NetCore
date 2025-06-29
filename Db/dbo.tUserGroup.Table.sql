USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tUserGroup]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tUserGroup](
	[ID] [uniqueidentifier] NOT NULL,
	[fkUserId] [nvarchar](50) NULL,
	[fkGroupId] [nvarchar](50) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tUserGroup] ADD  CONSTRAINT [DF_tUserGroup_ID]  DEFAULT (newid()) FOR [ID]
GO
