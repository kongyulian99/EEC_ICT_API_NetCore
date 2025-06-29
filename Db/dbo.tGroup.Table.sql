USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tGroup]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tGroup](
	[ID] [uniqueidentifier] NOT NULL,
	[cGroupCode] [nvarchar](50) NULL,
	[cGroupName] [nvarchar](250) NULL,
	[cStatus] [tinyint] NULL,
 CONSTRAINT [PK_tGroup] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tGroup] ADD  CONSTRAINT [DF_tGroup_ID]  DEFAULT (newid()) FOR [ID]
GO
