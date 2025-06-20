USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tGroupRole]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tGroupRole](
	[ID] [uniqueidentifier] NOT NULL,
	[fkGroupId] [nvarchar](50) NULL,
	[fkRoleId] [nvarchar](50) NULL,
 CONSTRAINT [PK_tGroupRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tGroupRole] ADD  CONSTRAINT [DF_tGroupRole_ID]  DEFAULT (newid()) FOR [ID]
GO
