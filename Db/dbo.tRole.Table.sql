USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tRole]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tRole](
	[ID] [uniqueidentifier] NOT NULL,
	[cRoleCode] [varchar](50) NULL,
	[cRoleName] [nvarchar](200) NULL,
	[cStatus] [tinyint] NULL,
 CONSTRAINT [PK_tRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tRole] ADD  CONSTRAINT [DF_tRole_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[tRole] ADD  CONSTRAINT [DF_tRole_cStatus]  DEFAULT ((1)) FOR [cStatus]
GO
