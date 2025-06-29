USE [TestDbHungND]
GO
/****** Object:  Table [dbo].[tUser]    Script Date: 5/26/2024 2:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tUser](
	[ID] [uniqueidentifier] NOT NULL,
	[cUserName] [varchar](50) NOT NULL,
	[cUserFullName] [varchar](50) NOT NULL,
	[cPassword] [nvarchar](200) NULL,
	[cSalt] [nvarchar](50) NULL,
	[cForceLogoutKey] [nvarchar](50) NULL,
	[cStatus] [tinyint] NULL,
 CONSTRAINT [PK_tUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tUser] ADD  CONSTRAINT [DF_tUser_ID]  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[tUser] ADD  CONSTRAINT [DF_tUser_cStatus]  DEFAULT ((1)) FOR [cStatus]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Key dùng để đưa vào mã hóa mật khẩu' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tUser', @level2type=N'COLUMN',@level2name=N'cSalt'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trường hợp muốn force logout user thì cập nhật chỗi key này thành key mới!' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tUser', @level2type=N'COLUMN',@level2name=N'cForceLogoutKey'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trạng thái active user (0: inactive, 1: active)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tUser', @level2type=N'COLUMN',@level2name=N'cStatus'
GO
