USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tMappingCommand]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tMappingCommand](
	[ID] [uniqueidentifier] NOT NULL,
	[cCommand] [nvarchar](250) NULL,
	[fkRoleId] [nvarchar](50) NULL,
	[fkPermissionId] [nvarchar](50) NULL,
 CONSTRAINT [PK_tMappingCommand] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tMappingCommand] ADD  CONSTRAINT [DF_tMappingCommand_ID]  DEFAULT (newid()) FOR [ID]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Mã request truyền từ client lên api' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tMappingCommand', @level2type=N'COLUMN',@level2name=N'cCommand'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Quyền phải có để thực thi tương ứng với mã request cCommand' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tMappingCommand', @level2type=N'COLUMN',@level2name=N'fkRoleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hành động View, add, update, delete... tương ứng với quyền' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tMappingCommand', @level2type=N'COLUMN',@level2name=N'fkPermissionId'
GO
