USE [master]
GO
/****** Object:  Database [ATWKAppDbFinal_TEST]    Script Date: 2/20/2018 11:57:29 PM ******/
CREATE DATABASE [ATWKAppDbFinal_TEST]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PostAppQuery2', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.ATWKDB\MSSQL\DATA\ATWKAppDbFinal_TEST1.mdf' , SIZE = 336064KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PostAppQuery2_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.ATWKDB\MSSQL\DATA\ATWKAppDbFinal_TEST1_log.ldf' , SIZE = 11200KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ATWKAppDbFinal_TEST].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ARITHABORT OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET  MULTI_USER 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET QUERY_STORE = OFF
GO
USE [ATWKAppDbFinal_TEST]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [ATWKAppDbFinal_TEST]
GO
/****** Object:  User [myDBUser]    Script Date: 2/20/2018 11:57:30 PM ******/
CREATE USER [myDBUser] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[myDBUser]
GO
/****** Object:  User [AppAccessor]    Script Date: 2/20/2018 11:57:30 PM ******/
CREATE USER [AppAccessor] FOR LOGIN [AppAccessor] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [myDBUser]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [myDBUser]
GO
ALTER ROLE [db_datareader] ADD MEMBER [myDBUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [myDBUser]
GO
/****** Object:  Schema [myDBUser]    Script Date: 2/20/2018 11:57:30 PM ******/
CREATE SCHEMA [myDBUser]
GO
/****** Object:  UserDefinedFunction [dbo].[CSVtoTable]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CSVtoTable]
(
    @LIST varchar(7000),
    @Delimeter varchar(10)
)
RETURNS @RET1 TABLE (RESULT BIGINT)
AS
BEGIN
    DECLARE @RET TABLE(RESULT BIGINT)
    
    IF LTRIM(RTRIM(@LIST))='' RETURN  

    DECLARE @START BIGINT
    DECLARE @LASTSTART BIGINT
    SET @LASTSTART=0
    SET @START=CHARINDEX(@Delimeter,@LIST,0)

    IF @START=0
    INSERT INTO @RET VALUES(SUBSTRING(@LIST,0,LEN(@LIST)+1))

    WHILE(@START >0)
    BEGIN
        INSERT INTO @RET VALUES(SUBSTRING(@LIST,@LASTSTART,@START-@LASTSTART))
        SET @LASTSTART=@START+1
        SET @START=CHARINDEX(@Delimeter,@LIST,@START+1)
        IF(@START=0)
        INSERT INTO @RET VALUES(SUBSTRING(@LIST,@LASTSTART,LEN(@LIST)+1))
    END
    
    INSERT INTO @RET1 SELECT * FROM @RET
    RETURN 
END

GO
/****** Object:  UserDefinedFunction [dbo].[DecryptString]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[DecryptString]
(
@passphrase varchar(50),
@encryptedText varbinary(max)
)
Returns
varchar(100)
as
begin

return CONVERT(varchar(100), DecryptByPassphrase(@passphrase,@encryptedText));

end

GO
/****** Object:  UserDefinedFunction [dbo].[EncryptString]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[EncryptString]
(
@passphrase varchar(50),
@text varchar(100)
)
Returns
varbinary(max)
as
begin

return EncryptByPassphrase(@passphrase,@text);

end

GO
/****** Object:  UserDefinedFunction [dbo].[GetUsersNameByUsername]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[GetUsersNameByUsername]
(
@username varchar(500)
)
Returns
varchar(500)
as
begin

declare @name varchar(500) ='';

select @name=b.Name from users a
join userprofile b on a.UserID= b.UserID and a.username=@username

return @name

end

GO
/****** Object:  Table [dbo].[CommonMessages]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommonMessages](
	[ID] [bigint] IDENTITY(1000,1) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[SentFrom] [varchar](200) NULL,
	[Status] [varchar](10) NULL,
	[IsNotificationSent] [varchar](10) NULL,
	[createdBy] [int] NULL,
	[modifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[UserID] [int] NULL,
	[subject] [nvarchar](500) NULL,
	[message] [nvarchar](max) NULL,
	[feedbackStatus] [varchar](4) NULL,
	[createdBy] [int] NULL,
	[modifiedBy] [int] NULL,
	[createdate] [datetime] NULL,
	[modifiedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Messages]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[SendTo] [varchar](500) NULL,
	[SentFrom] [varchar](500) NULL,
	[ReplyToID] [int] NULL,
	[Status] [varchar](10) NULL,
	[IsRead] [char](1) NULL,
	[createdBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsReplied] [char](1) NULL,
	[IsVoice] [varchar](1) NULL,
	[Subject] [nvarchar](max) NULL,
	[ayatollah] [nvarchar](max) NULL,
	[Remarks] [nvarchar](500) NULL,
	[ContentType] [varchar](100) NULL,
	[FileSize] [decimal](12, 2) NULL,
	[FileThumbnailId] [bigint] NULL,
	[FileTitle] [nvarchar](2000) NULL,
	[FileContextText] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessagesBackup]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessagesBackup](
	[Text] [varchar](max) NULL,
	[SendTo] [varchar](500) NULL,
	[SentFrom] [varchar](500) NULL,
	[ReplyToID] [int] NULL,
	[Status] [varchar](10) NULL,
	[IsRead] [char](1) NULL,
	[createdBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsReplied] [char](1) NULL,
	[IsVoice] [varchar](1) NULL,
	[Subject] [varchar](max) NULL,
	[ayatollah] [varchar](max) NULL,
	[ID] [int] NULL,
	[AUTOID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MessagesBackup8Jan2017]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessagesBackup8Jan2017](
	[ID] [int] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[SendTo] [varchar](500) NULL,
	[SentFrom] [varchar](500) NULL,
	[ReplyToID] [int] NULL,
	[Status] [varchar](10) NULL,
	[IsRead] [char](1) NULL,
	[createdBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsReplied] [char](1) NULL,
	[IsVoice] [varchar](1) NULL,
	[Subject] [nvarchar](max) NULL,
	[ayatollah] [nvarchar](max) NULL,
	[Remarks] [nvarchar](500) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[profileImages]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[profileImages](
	[imageID] [bigint] IDENTITY(1000,1) NOT NULL,
	[imageData] [varbinary](max) NULL,
	[imageExtension] [varchar](15) NULL,
	[createdBy] [int] NULL,
	[modifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[imageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblFiles]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblFiles](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[FileName] [varchar](max) NULL,
	[ContentType] [varchar](100) NULL,
	[Data] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[userProfile]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[userProfile](
	[UserProfileID] [bigint] IDENTITY(1000,1) NOT NULL,
	[userID] [int] NULL,
	[name] [nvarchar](500) NULL,
	[mobileNumber] [varchar](20) NULL,
	[location] [varchar](200) NULL,
	[nationality] [varchar](200) NULL,
	[createdBy] [int] NULL,
	[modifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifedDate] [datetime] NULL,
	[imageID] [bigint] NULL,
	[Details] [varchar](max) NULL,
	[Age] [decimal](5, 2) NULL,
	[Gender] [varchar](10) NULL,
	[Language] [varchar](50) NULL,
	[SpecialisationIn] [nvarchar](max) NULL,
	[StudiesAt] [nvarchar](max) NULL,
	[TypicallyRepliesIn] [nvarchar](50) NULL,
	[archiveUrl] [nvarchar](2000) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserProfileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[users]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[userID] [int] IDENTITY(1000,1) NOT NULL,
	[username] [varchar](100) NULL,
	[userType] [varchar](30) NULL,
	[password] [varbinary](max) NULL,
	[passphrase] [varchar](100) NULL,
	[userStatus] [varchar](10) NULL,
	[createdBy] [int] NULL,
	[modifiedBy] [int] NULL,
	[createdDate] [datetime] NULL,
	[modifiedDate] [datetime] NULL,
	[lastLoginTimeStamp] [datetime] NULL,
	[userLoginStatus] [varchar](10) NULL,
	[lastOnlineTime] [datetime] NULL,
	[RegistrationID] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Messages] ADD  DEFAULT ('') FOR [Remarks]
GO
/****** Object:  StoredProcedure [dbo].[activateUser]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[activateUser]
(
@passphrase varchar(50),
@userID int
)
as
begin

--set nocount on;

update users 
set
UserStatus='ACTV'
where
userID=@userID and passphrase =@passphrase
and UserStatus !='DEL'
end

GO
/****** Object:  StoredProcedure [dbo].[AddDocumentMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[AddDocumentMessage]
(
@SendTo varchar(500),
@SentFrom varchar(500),
@ReplyToID int,
@Status varchar(10),
@IsRead char(1),
@createdBy int,
@FileName varchar(max),
@ContentType varchar(100),
@Data varbinary(max),
@Subject nvarchar(max),
@Ayatollah nvarchar(max),
@thumbNail varbinary(max),
@fileContextText nvarchar(max)
)
as
begin

DECLARE @id int;
DECLARE @tblFileID int;
DECLARE @tblthumbFileID int;
DECLARE @table table (id int);

DECLARE @tblFileIDTable table (id int);
DECLARE @tblthumbFileIDTable table (id int)


INSERT INTO tblFiles
           (FileName
           ,ContentType
           ,Data)
            OUTPUT inserted.ID into @tblFileIDTable
     VALUES
           (@FileName
           ,@ContentType
           ,@Data)

  SELECT @tblFileID = id from @tblFileIDTable;

  INSERT INTO tblFiles
           (FileName
           ,ContentType
           ,Data)
            OUTPUT inserted.ID into @tblthumbFileIDTable
     VALUES
           (@FileName +'.jpg'
           ,'image/jpg'
           ,@thumbNail);

SELECT @tblthumbFileID = id from @tblthumbFileIDTable;

INSERT INTO Messages
           (Text
           ,SendTo
           ,SentFrom
           ,ReplyToID
           ,Status
           ,IsRead
           ,createdBy
           ,ModifiedBy
           ,createdDate
           ,ModifiedDate
           ,IsVoice
           ,Subject,IsReplied,Ayatollah,ContentType,FileSize,FileTitle,FileThumbnailId,FileContextText)
            OUTPUT inserted.ID into @table
     VALUES
          (@tblFileID
           ,@SendTo
           ,@SentFrom
           ,@ReplyToID
           ,@Status
           ,@IsRead
           ,@createdBy
           ,@createdBy
           ,getdate()
           ,getdate()
           ,'N'
           ,@Subject,'N',@Ayatollah,@ContentType ,DATALENGTH(@Data),@FileName,@tblthumbFileID,@fileContextText
          )
          
    
           SELECT @id = id from @table;

          if @ReplyToID <>0
          begin
          update Messages set IsReplied='Y'
          where ID=@ReplyToID;
          end
          --else
          --begin
          --update Messages set IsReplied='N'
          --where ID=@id;
          --end

--select @tblFileID as 'FileID';

sELECT @tblFileID;


end


GO
/****** Object:  StoredProcedure [dbo].[AddFeedback]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[AddFeedback]
(
@userid int,
@subject nvarchar(500),
@message nvarchar(max)
)
as
begin


insert into feedback
(
UserID ,
subject ,
message ,
feedbackStatus ,
createdBy ,
modifiedBy ,
createdate ,
modifiedDate 
)
values
(
@userid ,
@subject ,
@message ,
'ACTV' ,
@UserID ,
0 ,
getdate() ,
getdate() 
);
end

GO
/****** Object:  StoredProcedure [dbo].[AddMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[AddMessage]
(
@Text nvarchar(max),
@SendTo varchar(500),
@SentFrom varchar(500),
@ReplyToID int,
@Status varchar(10),
@IsRead char(1),
@createdBy int,
@Subject nvarchar(max),
@Ayatollah nvarchar(max)
)
as
begin

DECLARE @id int;
DECLARE @table table (id int);

--INSERT INTO Messages
--           (Text
--           ,SendTo
--           ,SentFrom
--           ,ReplyToID
--           ,Status
--           ,IsRead
--           ,createdBy
--           ,ModifiedBy
--           ,createdDate
--           ,ModifiedDate
--           ,IsVoice
--           ,Subject,IsReplied)
--            OUTPUT inserted.ID into @table
--     VALUES
--          (@Text
--           ,@SendTo
--           ,@SentFrom
--           ,@ReplyToID
--           ,@Status
--           ,@IsRead
--           ,@createdBy
--           ,@createdBy
--           ,getdate()
--           ,getdate()
--           ,'N'
--           ,@Subject,'N'
--          )

INSERT INTO Messages
           (Text
           ,SendTo
           ,SentFrom
           ,ReplyToID
           ,Status
           ,IsRead
           ,createdBy
           ,ModifiedBy
           ,createdDate
           ,ModifiedDate
           ,IsVoice
           ,Subject,IsReplied,Ayatollah)
            OUTPUT inserted.ID into @table
     VALUES
          (@Text
           ,@SendTo
           ,@SentFrom
           ,@ReplyToID
           ,@Status
           ,@IsRead
           ,@createdBy
           ,@createdBy
           ,getdate()
           ,getdate()
           ,'N'
           ,@Subject,'N',@Ayatollah
          )
          
          
          
           SELECT @id = id from @table;

          
          if @ReplyToID <>0
          begin
          update Messages set IsReplied='Y'
          where ID=@ReplyToID;
          end
          --else
          --begin
          --update Messages set IsReplied='N'
          --where ID=@id;
          --end

end

GO
/****** Object:  StoredProcedure [dbo].[AddVoiceMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[AddVoiceMessage]
(
@SendTo varchar(500),
@SentFrom varchar(500),
@ReplyToID int,
@Status varchar(10),
@IsRead char(1),
@createdBy int,
@FileName varchar(max),
@ContentType varchar(100),
@Data varbinary(max),
@Subject nvarchar(max),
@Ayatollah nvarchar(max)
)
as
begin

DECLARE @id int;
DECLARE @tblFileID int;
DECLARE @table table (id int);

DECLARE @tblFileIDTable table (id int)


INSERT INTO tblFiles
           (FileName
           ,ContentType
           ,Data)
            OUTPUT inserted.ID into @tblFileIDTable
     VALUES
           (@FileName
           ,@ContentType
           ,@Data)

  SELECT @tblFileID = id from @tblFileIDTable;

INSERT INTO Messages
           (Text
           ,SendTo
           ,SentFrom
           ,ReplyToID
           ,Status
           ,IsRead
           ,createdBy
           ,ModifiedBy
           ,createdDate
           ,ModifiedDate
           ,IsVoice
           ,Subject,IsReplied,Ayatollah)
            OUTPUT inserted.ID into @table
     VALUES
          (@tblFileID
           ,@SendTo
           ,@SentFrom
           ,@ReplyToID
           ,@Status
           ,@IsRead
           ,@createdBy
           ,@createdBy
           ,getdate()
           ,getdate()
           ,'Y'
           ,@Subject,'N',@Ayatollah
          )
          
    
           SELECT @id = id from @table;

          if @ReplyToID <>0
          begin
          update Messages set IsReplied='Y'
          where ID=@ReplyToID;
          end
          --else
          --begin
          --update Messages set IsReplied='N'
          --where ID=@id;
          --end

--select @tblFileID as 'FileID';

sELECT @tblFileID;


end

GO
/****** Object:  StoredProcedure [dbo].[authenticateUser]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[authenticateUser]
(
@username varchar(100),
@password varchar(100)
)
as
begin

select * from users where  Upper(username) =@username
and dbo.DecryptString(passphrase,password)=@password and userstatus !='DEL'

end

GO
/****** Object:  StoredProcedure [dbo].[bc_GetChatHistoryUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[bc_GetChatHistoryUsers]  
(  
@userID int  
)  
as  
begin  
declare @username varchar(500);  
declare @userType varchar(20);  
  
select @username=username,@userType=userType from users where userID=@userID;  
IF @userType <>'MODERATOR'  
BEGIN  
	select a.UserID,a.Username,  
	a.UserType,a.UserloginStatus,  
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt into #UserDetailsForUser from Users a  
	join userProfile b on   
	a.UserID=b.UserID   
	join messages ms on ms.sendto=a.username
	and a.userStatus='ACTV'
	and ms.sentfrom=@username order by ms.createddate desc
 
	select distinct UserID,Username,  
	UserType,UserloginStatus,  
	Name,Location,Nationality,Details,Age,Gender,ImageID,lastOnlineTime,language,SpecialisationIn,StudiesAt from #UserDetailsForUser
 
	drop table #UserDetailsForUser  
END  
ELSE  
BEGIN  
	select a.UserID,a.Username,  
	a.UserType,a.UserloginStatus,  
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt into #UserDetailsModiator from Users a  
	join userProfile b on   
	a.UserID=b.UserID   
	join messages ms on ms.sendto=a.username
	and a.userStatus='ACTV'
	order by ms.createddate desc
 
	select distinct UserID,Username,  
	UserType,UserloginStatus,  
	Name,Location,Nationality,Details,Age,Gender,ImageID,lastOnlineTime,language,SpecialisationIn,StudiesAt from #UserDetailsModiator
 
	drop table #UserDetailsModiator   
END  
  
end

GO
/****** Object:  StoredProcedure [dbo].[ChangePassword]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ChangePassword]
(
@userid int,
@newpassword varchar(100),
@modifiedBy int
)
as
begin
declare @passphrase varchar(100)=newID();

update users
set
password =dbo.EncryptString(@passphrase,@newpassword),
passphrase=@passphrase,
modifiedBy=@modifiedBy,
modifiedDate=getdate()
where userID =@userid
end

GO
/****** Object:  StoredProcedure [dbo].[CheckPasswordValidity]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[CheckPasswordValidity]
(
@userid int,
@password varchar(100)
)
as
begin
select * from users where userID=@userid and dbo.DecryptString(passphrase,password)=@password;
end

GO
/****** Object:  StoredProcedure [dbo].[deActivateUser]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[deActivateUser]  
(  
@userID int  
)  
as  
begin  
update users   
set userStatus='IACT'  ,userLoginStatus='OFFLINE'
where userID= @userID;  
end

GO
/****** Object:  StoredProcedure [dbo].[deleteUser]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[deleteUser]
(
@userID int,
@indc int
)
as
begin

-- for temp delete make @indc=0

if(@indc=1)
begin
update users set userStatus='DEL' where userID =@userID;
end
else
begin
declare @imageID int =0;
declare @username varchar(500);

select @username=username from users where userID= @userID;
select @imageID=imageID from userProfile where userID= @userID;
delete from users where userID= @userID;
delete from userProfile where userID= @userID;
delete from tblFiles where ID= @imageID;
delete from Messages where SentFrom=@username
end

end
GO
/****** Object:  StoredProcedure [dbo].[GeModelPushNotificationDetails]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GeModelPushNotificationDetails]
	@FMailId varchar(500),
	@TMailId varchar(500)
AS
BEGIN
	select distinct u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where
m.SentFrom=@FMailId and m.SendTo=@TMailId
END

GO
/****** Object:  StoredProcedure [dbo].[GetAllAccesors]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllAccesors]    
(  
@userType varchar(10)  
)  
as    
begin    
    
select a.UserID,a.UserName,a.UserType,a.UserStatus,a.UserloginStatus,b.name,b.mobileNumber,b.UserProfileID, a.createdBy,a.modifiedBy,a.createdDate,a.modifiedDate,a.lastOnlineTime,
b.location,b.nationality,b.Age,b.Details,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt,ISNULL(b.TypicallyRepliesIn, '') as TypicallyRepliesIn
,ISNULL(b.archiveUrl, '') as archiveUrl from users a  
left join userProfile b on  
a.UserID =b.UserID   
where a.userType=@userType   
and a.userStatus <>'DEL'  
ORDER BY b.Name    
    
end  
GO
/****** Object:  StoredProcedure [dbo].[GetAllALims]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllALims]
as
begin

select a.UserID,a.Username,
a.UserType,a.UserloginStatus,
b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
join userProfile b on 
a.UserID=b.UserID and a.userType='ALIM'
and a.userStatus='ACTV'
ORDER BY a.userLoginStatus desc ,a.lastOnlineTime desc,b.name asc
end

GO
/****** Object:  StoredProcedure [dbo].[GetAllALimsByCountry]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllALimsByCountry]
(
@location varchar(200)
)
as
begin
select a.UserID,a.Username,
a.UserType,a.UserloginStatus,
b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
join userProfile b on 
a.UserID=b.UserID and a.userType='ALIM'
and a.userStatus='ACTV' and b.Location=@location

end

GO
/****** Object:  StoredProcedure [dbo].[GetAllMessages]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetAllMessages]
as
begin

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
order by m.createdDate asc
end

GO
/****** Object:  StoredProcedure [dbo].[GetAllMessagesById]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllMessagesById]
(
@userId bigint
)
as
begin

declare @Me varchar(200); 
select @Me=username from users where userID =@userId;

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID,
case when m.SendTo=@Me then m.SentFrom 
when m.SentFrom=@Me then m.SendTo end as OtherParty
 from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (sendto=@Me and m.Status='APPROVE') or SentFrom=@Me
order by OtherParty asc, m.createdDate asc

end


GO
/****** Object:  StoredProcedure [dbo].[GetAllModeators]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetAllModeators] 
	
AS
BEGIN
	select * from users where userstatus='ACTV' and userType='MODERATOR' and Registrationid is not null

END

GO
/****** Object:  StoredProcedure [dbo].[GetAllUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetAllUsers]
as
begin

select a.UserID,a.Username,
a.UserType,a.UserloginStatus,
b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
join userProfile b on 
a.UserID=b.UserID and a.userType='USER'
and a.userStatus='ACTV'
ORDER BY a.userLoginStatus desc ,a.lastOnlineTime desc,b.name asc

end

GO
/****** Object:  StoredProcedure [dbo].[GetChatHistoryUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetChatHistoryUsers]  
(    
@userID int    
)    
as    
begin    
declare @username varchar(500);    
declare @userType varchar(20);    
    
select @username=username,@userType=userType from users where userID=@userID;    
IF @userType <>'MODERATOR'    
BEGIN    
 select a.UserID,a.Username,    
 a.UserType,a.UserloginStatus,    
 b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a    
 join userProfile b on     
 a.UserID=b.UserID     
 and a.userStatus='ACTV' and a.UserName in    
 (    
 select  distinct x from    
 (    
 select SendTo as x from messages where SentFrom = @username  and Status='APPROVE'  
 union    
 select SentFrom as x from messages where SendTo = @username  and Status='APPROVE'  
 ) xc    
 )    
END    
ELSE    
BEGIN    
   select   
 'MODERATOR' as 'QueryBy',  
  P1.Name as 'UsersName',  
  P1.ImageID as 'UsersImageID',  
  U1.UserName as 'UsersUserName',  
  P1.UserID as 'UsersUserID',  
  P2.Name as 'ScholarsName',  
  P2.ImageID as 'ScholarsImageID',  
  U2.UserName as 'ScholarsUserName',  
  P2.UserID as 'ScholarsUserID'  ,
  max(tb.z) as test
   from  
  (  
  select distinct sendTo as x,sentFrom as y,MAX(ID) as z  from messages where sentFrom in (select username from users where userType ='ALIM')  group By sendTo,sentFrom
  union  
  select distinct sentFrom as x,sendTo as y,MAX(ID) as z from messages where sendTo in (select username from users where userType ='ALIM')  group By sentFrom,sendTo
  )tb   
  join Users U1 on U1.username=tb.x join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'  
  join Users U2 on U2.username=tb.y join userProfile P2 on P2.userID=U2.userID and U2.UserStatus='ACTV' 
   group by P1.Name,P2.Name,P1.ImageID,U1.UserName,P1.UserID,P2.ImageID,U2.UserName,P2.UserID
  order by test desc
END    
    
end  
GO
/****** Object:  StoredProcedure [dbo].[GetChatHistoryUsers_backUp]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetChatHistoryUsers_backUp]
(  
@userID int  
)  
as  
begin  
declare @username varchar(500);  
declare @userType varchar(20);  
  
select @username=username,@userType=userType from users where userID=@userID;  
IF @userType <>'MODERATOR'  
BEGIN  
 select a.UserID,a.Username,  
 a.UserType,a.UserloginStatus,  
 b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a  
 join userProfile b on   
 a.UserID=b.UserID   
 and a.userStatus='ACTV' and a.UserName in  
 (  
 select  distinct x from  
 (  
 select SendTo as x from messages where SentFrom = @username  
 union  
 select SentFrom as x from messages where SendTo = @username  
 ) xc  
 )  
END  
ELSE  
BEGIN  
 select a.UserID,a.Username,  
 a.UserType,a.UserloginStatus,  
 b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a  
 join userProfile b on   
 a.UserID=b.UserID   
 and a.userStatus='ACTV' and a.UserName in  
 (  
 select x from  
 (  
 select SendTo as x from messages   
 union  
 select SentFrom as x from messages  
 ) xc  
 )  
END  
  
end

GO
/****** Object:  StoredProcedure [dbo].[GetCommonMessages]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[GetCommonMessages]
as
begin

select * from CommonMessages where Status ='ACTV';
end
GO
/****** Object:  StoredProcedure [dbo].[GetConversationTree]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetConversationTree]  
(  
@Me varchar(200),  
@You varchar(200)  
)  
as  
begin  
  
declare @userType varchar(20);  
  
select @userType=userType from users where username =@Me  
  
IF @userType <>'MODERATOR'  
BEGIN  
 select m.*,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,u1.userType as [FromUserType] from messages m   
join users u1 on u1.username=m.SentFrom  
join users u2 on u2.username=m.SendTo  
join userProfile p1 on p1.userID=u1.userID  
join userProfile p2 on p2.userID=u2.userID  
where (m.sendTo=@You and m.sentFrom=@Me )  
 or (m.sendTo=@Me and m.sentFrom=@You and m.Status='APPROVE')  
 order by m.createdDate asc  
END  
ELSE  
BEGIN  
 select m.*,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,u1.userType as [FromUserType] from messages m   
join users u1 on u1.username=m.SentFrom  
join users u2 on u2.username=m.SendTo  
join userProfile p1 on p1.userID=u1.userID  
join userProfile p2 on p2.userID=u2.userID  
where (m.sentFrom=@You )  
 or (m.sendTo=@You and m.Status='APPROVE')  
 order by m.createdDate asc  
  
END  
  
  
end

GO
/****** Object:  StoredProcedure [dbo].[GetConversationTreeById]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetConversationTreeById]  
(  
@MeId int,  
@YouId int  
)  
as  
begin  
  
declare @userType varchar(20);  
declare @Me varchar(200); 
declare @You varchar(200); 

select @Me=username from users where userID =@MeId;
select @You=username from users where userID =@YouId;
  
select @userType=userType from users where username =@Me  
  
IF @userType <>'MODERATOR'  
BEGIN  
 select m.*,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,u1.userType as [FromUserType],
case when m.SentFrom =@Me then 'Y' else 'N'  end as IsMine
from messages m   
join users u1 on u1.username=m.SentFrom  
join users u2 on u2.username=m.SendTo  
join userProfile p1 on p1.userID=u1.userID  
join userProfile p2 on p2.userID=u2.userID  
where (m.sendTo=@You and m.sentFrom=@Me )  
 or (m.sendTo=@Me and m.sentFrom=@You and m.Status='APPROVE')  
 order by m.createdDate asc  
END  
ELSE  
BEGIN  
 select m.*,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,u1.userType as [FromUserType],
case when m.SentFrom =@You then 'Y' else 'N'  end as IsMine
from messages m   
join users u1 on u1.username=m.SentFrom  
join users u2 on u2.username=m.SendTo  
join userProfile p1 on p1.userID=u1.userID  
join userProfile p2 on p2.userID=u2.userID  
where (m.sentFrom=@You )  
 or (m.sendTo=@You and m.Status='APPROVE')  
 order by m.createdDate asc  
  
END  
  
  
end

GO
/****** Object:  StoredProcedure [dbo].[GetData]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetData]
(
@ID int
)
as
begin

select * from tblFiles where ID=@ID;

end

GO
/****** Object:  StoredProcedure [dbo].[GetHomePageDetails]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetHomePageDetails]
as
begin
declare @alimCount int;
declare @userCount int;
declare @messageCount int;
declare @moderatorCount int;

select @alimCount=count(*) from users where userStatus !='DEL' and UserType= 'ALIM'
select @userCount=count(*) from users where userStatus !='DEL' and UserType= 'USER'
select @moderatorCount=count(*) from users where userStatus !='DEL' and UserType= 'MODERATOR'
select @messageCount=count(*) from Messages

select @alimCount as 'ALIM',@userCount as 'USER',@messageCount as 'MESSAGE',@moderatorCount as 'MODERATOR'

end

GO
/****** Object:  StoredProcedure [dbo].[GetIncomingMessagesByUserID]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetIncomingMessagesByUserID]
(
@userID int
)
as
begin

declare @username varchar(500);

select @username=username from users where userID =@userID;

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where m.sendTo=@username and Status='APPROVE'
order by m.createdDate asc

end

GO
/****** Object:  StoredProcedure [dbo].[GetIncomingMessagesByUserName]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetIncomingMessagesByUserName]
(
@Me varchar(200),
@You varchar(200)
)
as
begin

declare @userType varchar(20);

select @userType=userType from users where username =@Me


IF @userType <>'MODERATOR'
BEGIN
	select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (m.sendTo=@Me and m.sentFrom=@You and m.Status='APPROVE')
	order by m.createdDate asc
END
ELSE
BEGIN

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (m.SendTo=@You and m.Status='APPROVE' )
	order by m.createdDate asc

END


end

GO
/****** Object:  StoredProcedure [dbo].[GetIncomingMessageUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetIncomingMessageUsers]
(
@userID int
)
as
begin

declare @username varchar(500);
declare @userType varchar(20);

select @username=username,@userType=userType from users where userID=@userID;

IF @userType <>'MODERATOR'
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
(select count(*) from Messages where IsRead='N' and SentFrom=a.Username and SendTo=@username) as unreadMessages,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select distinct SentFrom from messages where SendTo = @username
	)
END
ELSE
BEGIN
select 
	'MODERATOR' as 'QueryBy',
	 P1.Name as 'UsersName',
	 P1.ImageID as 'UsersImageID',
	 U1.UserName as 'UsersUserName',
	 P1.UserID as 'UsersUserID',
	 P2.Name as 'ScholarsName',
	 P2.ImageID as 'ScholarsImageID',
	 U2.UserName as 'ScholarsUserName',
	 P2.UserID as 'ScholarsUserID',
	 (select count(*) from Messages where IsRead='N' and 
	(SentFrom=U1.UserName and SendTo=U2.UserName)
	or
	(SentFrom=U2.UserName and SendTo=U1.UserName)
	)
	 as unreadMessages
	  from
	 (
	 select distinct sendTo as x,sentFrom as y from messages where sentFrom in (select username from users where userType ='ALIM')
	 union
	 select distinct sentFrom as x,sendTo as y from messages where sendTo in (select username from users where userType ='ALIM')
	 )tb 
	 join Users U1 on U1.username=tb.x join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'
	 join Users U2 on U2.username=tb.y join userProfile P2 on P2.userID=U2.userID and U2.UserStatus='ACTV'
END

end


GO
/****** Object:  StoredProcedure [dbo].[GetIncomingMessageUsers_backup]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetIncomingMessageUsers_backup]
(
@userID int
)
as
begin

declare @username varchar(500);
declare @userType varchar(20);

select @username=username,@userType=userType from users where userID=@userID;

IF @userType <>'MODERATOR'
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select distinct SentFrom from messages where SendTo = @username
	)
END
ELSE
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select  distinct x from
	(
	select SendTo as x from messages 
	union
	select SentFrom as x from messages
	) xc
	)
END

end

GO
/****** Object:  StoredProcedure [dbo].[GetIncomingMessageUsers_backup2]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetIncomingMessageUsers_backup2]
(
@userID int
)
as
begin

declare @username varchar(500);
declare @userType varchar(20);

select @username=username,@userType=userType from users where userID=@userID;

IF @userType <>'MODERATOR'
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select distinct SentFrom from messages where SendTo = @username
	)
END
ELSE
BEGIN
select 
	'MODERATOR' as 'QueryBy',
	 P1.Name as 'UsersName',
	 P1.ImageID as 'UsersImageID',
	 U1.UserName as 'UsersUserName',
	 P1.UserID as 'UsersUserID',
	 P2.Name as 'ScholarsName',
	 P2.ImageID as 'ScholarsImageID',
	 U2.UserName as 'ScholarsUserName',
	 P2.UserID as 'ScholarsUserID'
	  from
	 (
	 select distinct sendTo as x,sentFrom as y from messages where sentFrom in (select username from users where userType ='ALIM')
	 union
	 select distinct sentFrom as x,sendTo as y from messages where sendTo in (select username from users where userType ='ALIM')
	 )tb 
	 join Users U1 on U1.username=tb.x join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'
	 join Users U2 on U2.username=tb.y join userProfile P2 on P2.userID=U2.userID and U2.UserStatus='ACTV'
END

end

GO
/****** Object:  StoredProcedure [dbo].[GetMessageByStatus]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetMessageByStatus]
(
@Status varchar(10)
)
as
begin

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where m.Status =@Status
order by m.createdDate asc
end

GO
/****** Object:  StoredProcedure [dbo].[GetModiatorNotification]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetModiatorNotification]
AS
BEGIN
	select [ID]
      ,[Text]
      ,[SendTo]
      ,[SentFrom]
      ,[ReplyToID]
      ,[Status]
      ,[IsRead]
      ,m.[createdBy]
      ,m.[ModifiedBy]
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as createdDate
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as ModifiedDate
      ,[IsReplied]
      ,[IsVoice]
      ,[Subject],
      ayatollah,u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
	where m.status='PEND' order by createddate desc
END


GO
/****** Object:  StoredProcedure [dbo].[GetOnlineUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetOnlineUsers]
as
begin

select p.name,a.RegistrationID from users a join
userProfile p on a.userID=p.userID where a.RegistrationID is not null

end
GO
/****** Object:  StoredProcedure [dbo].[GetOutGoingMessagesByUserID]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetOutGoingMessagesByUserID]
(
@userID int
)
as
begin

declare @username varchar(500);

select @username=username from users where userID =@userID;

	select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where m.sentFrom=@username ;


end

GO
/****** Object:  StoredProcedure [dbo].[GetOutgoingMessagesByUserName]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetOutgoingMessagesByUserName]
(
@Me varchar(200),
@You varchar(200)
)
as
begin


declare @userType varchar(20);

select @userType=userType from users where username =@Me



IF @userType <>'MODERATOR'
BEGIN
	select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (m.sendTo=@You and m.sentFrom=@Me )
	order by m.createdDate asc
END
ELSE
BEGIN

		select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (m.sentFrom=@You )
	order by m.createdDate asc

END




end

GO
/****** Object:  StoredProcedure [dbo].[GetOutgoingMessageUsers]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetOutgoingMessageUsers]
(
@userID int
)
as
begin

declare @username varchar(500);
declare @userType varchar(20);

select @username=username,@userType=userType from users where userID=@userID;

IF @userType <>'MODERATOR'
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt  from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select distinct SendTo from messages where SentFrom = @username
	)
END
ELSE
BEGIN
	select a.UserID,a.Username,
	a.UserType,a.UserloginStatus,
	b.Name,b.Location,b.Nationality,b.Details,b.Age,b.Gender,b.ImageID,a.lastOnlineTime  from Users a
	join userProfile b on 
	a.UserID=b.UserID 
	and a.userStatus='ACTV' and a.UserName in
	(
	select  distinct x from
	(
	select SendTo as x from messages 
	union
	select SentFrom as x from messages
	) xc
	)
END

end

GO
/****** Object:  StoredProcedure [dbo].[GetPassword]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetPassword]
(
@username varchar(100)
)
as
begin
select dbo.DecryptString(passphrase,password ) as password from users 
where username =@username and userStatus !='DEL'

end
GO
/****** Object:  StoredProcedure [dbo].[GetPendingQueryAlimsWithCount]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetPendingQueryAlimsWithCount]
as
begin
select 
 	 P1.Name as 'ScholarsName',
	 P1.ImageID as 'ScholarsImageID',
	 U1.UserName as 'ScholarsUserName',
	 P1.UserID as 'ScholarsUserID',
 unreadCount
 from 
(
select count(*) as unreadCount,sendTo from Messages where IsReplied='N' and Status='APPROVE'
and sendTo in (select username from users where userType ='ALIM')
group by sendTo
) a
join Users U1 on U1.username=a.SendTo join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'
end
GO
/****** Object:  StoredProcedure [dbo].[GetPendingQueryWCByAlim]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[GetPendingQueryWCByAlim]
(
@scholarUsername varchar(100)
)
as
begin
select 
 P2.Name as 'UsersName',
 P2.ImageID as 'UsersImageID',
 U2.UserName as 'UsersUserName',
 P2.UserID as 'UsersUserID',
 	 P1.Name as 'ScholarsName',
	 P1.ImageID as 'ScholarsImageID',
	 U1.UserName as 'ScholarsUserName',
	 P1.UserID as 'ScholarsUserID',
 unreadCount
 from 
(
select count(*) as unreadCount,sendTo,SentFrom from Messages where IsReplied='N' and Status='APPROVE'
and sendTo in (select username from users where userType ='ALIM' and username =@scholarUsername)
group by sendTo,SentFrom
) a
join Users U1 on U1.username=a.SendTo join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'
join Users U2 on U2.username=a.SentFrom join userProfile P2 on P2.userID=U2.userID and U2.UserStatus='ACTV'

end



GO
/****** Object:  StoredProcedure [dbo].[GetRegistrationID]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetRegistrationID]
(
@userId varchar(100),
@indc int
)
as
begin
if @indc=0
begin
select RegistrationID from users where userID=@userId;
end
else
begin
select RegistrationID from users where username=@userId;

end

end

GO
/****** Object:  StoredProcedure [dbo].[GetRegistrationScollerID]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetRegistrationScollerID] --1693
	@messageID int
AS
BEGIN
	select RegistrationID from users where username=(select SendTo from messages  where id=@messageID)
END

GO
/****** Object:  StoredProcedure [dbo].[GetSchollerPushNotificationDetails]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetSchollerPushNotificationDetails] 
	@id int
AS
BEGIN
	select [ID]
      ,[Text]
      ,[SendTo]
      ,[SentFrom]
      ,[ReplyToID]
      ,[Status]
      ,[IsRead]
      ,m.[createdBy]
      ,m.[ModifiedBy]
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as createdDate
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as ModifiedDate
      ,'Y' as [IsReplied]
      ,[IsVoice]
      ,[Subject],ayatollah,u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where id=@id --and u2.userLoginstatus='ONLINE'
END


GO
/****** Object:  StoredProcedure [dbo].[GetScollerNotification]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetScollerNotification]  
 @MailId varchar(max)  
AS  
BEGIN  
 select [ID]  
      ,[Text]  
      ,[SendTo]  
      ,[SentFrom]  
      ,[ReplyToID]  
      ,[Status]  
      ,[IsRead]  
      ,m.[createdBy]  
      ,m.[ModifiedBy]  
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as createdDate  
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as ModifiedDate  
      ,[IsReplied]  
      ,[IsVoice]  
      ,[Subject],ayatollah,u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID from messages m   
join users u1 on u1.username=m.SentFrom  
join users u2 on u2.username=m.SendTo  
join userProfile p1 on p1.userID=u1.userID  
join userProfile p2 on p2.userID=u2.userID   
where
id in (select id from (select max(id) as id ,Subject from messages m 
 where IsReplied='N'  
 and SendTo=@MailId  
 and m.ReplYTOiD=0  
 and m.status='APPROVE' group by Subject) a) order by createddate desc
END


GO
/****** Object:  StoredProcedure [dbo].[GetUnansweredAlims]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create procedure [dbo].[GetUnansweredAlims]
as
begin

 select   
 'MODERATOR' as 'QueryBy',  
  P1.Name as 'UsersName',  
  P1.ImageID as 'UsersImageID',  
  U1.UserName as 'UsersUserName',  
  P1.UserID as 'UsersUserID',  
  P2.Name as 'ScholarsName',  
  P2.ImageID as 'ScholarsImageID',  
  U2.UserName as 'ScholarsUserName',  
  P2.UserID as 'ScholarsUserID'  ,
  tb.messageID as messageID
   from  
  (  
 select distinct sendTo as x,sentfrom as y ,MAX(ID) as messageID  from Messages where IsReplied='N' and Status='APPROVE' and
  SendTo in (select username from users where userType ='ALIM')  group By sendTo,SentFrom
  )tb   
  join Users U1 on U1.username=tb.x join userProfile P1 on P1.userID=U1.userID and U1.UserStatus='ACTV'  
  join Users U2 on U2.username=tb.y join userProfile P2 on P2.userID=U2.userID and U2.UserStatus='ACTV' 
  order by messageID desc;
  
end
GO
/****** Object:  StoredProcedure [dbo].[GetUnansweredQueriesByUsername]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetUnansweredQueriesByUsername]
(
@usersUsername varchar(100),
@scholarUsername varchar(100)
)
as
begin

select m.*,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where (m.SendTo=@scholarUsername and m.Status='APPROVE' and m.IsReplied='N' and  (m.SentFrom=@usersUsername or @usersUsername='55555' ))
order by ModifiedDate desc ,createdDate desc

end


GO
/****** Object:  StoredProcedure [dbo].[GetUserByUsername]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetUserByUsername]
(
@username varchar(100)
)
as
begin

select * from users where username =@username
and userStatus !='DEL'

end
GO
/****** Object:  StoredProcedure [dbo].[GetUserDetails]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetUserDetails]
(
@userID int
)
as
begin

select a.UserID,a.UserName,a.UserType,a.UserStatus,a.UserloginStatus,b.name,b.mobileNumber,
b.location,b.nationality,b.Age,b.Details,b.Gender,b.ImageID,a.lastOnlineTime,b.language,b.SpecialisationIn,b.StudiesAt,ISNULL(b.TypicallyRepliesIn, '') as TypicallyRepliesIn
,ISNULL(b.archiveUrl, '') as archiveUrl from users a
left join userProfile b on
a.UserID =b.UserID where a.USerID=@userID
end
GO
/****** Object:  StoredProcedure [dbo].[GetUserDetailsByUsername]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[GetUserDetailsByUsername]
(
@username varchar(500)
)
as
begin
select a.UserID,a.username,a.usertype,a.userstatus,a.createdDate,a.modifiedDate,a.createdBy,a.modifiedBy,
a.lastLoginTimestamp,a.userloginstatus,a.lastonlinetime,b.UserProfileID,
b.name,b.mobilenumber,b.location,b.nationality,b.imageID,b.Details,b.Age,
b.Gender,b.language,b.SpecialisationIn,b.StudiesAt,ISNULL(b.TypicallyRepliesIn, '') as TypicallyRepliesIn
,ISNULL(b.archiveUrl, '') as archiveUrl from users a
join userprofile b on 
a.userID =b.userID
where a.username=@username
end
GO
/****** Object:  StoredProcedure [dbo].[GetUserNotification]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetUserNotification]
	@MailId varchar(max)
AS
BEGIN
	select [ID]
      ,[Text]
      ,[SendTo]
      ,[SentFrom]
      ,[ReplyToID]
      ,[Status]
      ,[IsRead]
      ,m.[createdBy]
      ,m.[ModifiedBy]
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as createdDate
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as ModifiedDate
      ,'Y' as [IsReplied]
      ,[IsVoice]
      ,[Subject],ayatollah,u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where IsRead='N'
	and SendTo=@MailId order by createddate desc
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserPushNotificationDetails]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[GetUserPushNotificationDetails] 
	@id int
AS
BEGIN
	select [ID]
      ,[Text]
      ,[SendTo]
      ,[SentFrom]
      ,[ReplyToID]
      ,[Status]
      ,[IsRead]
      ,m.[createdBy]
      ,m.[ModifiedBy]
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as createdDate
      ,substring(CONVERT(varchar,m.[createdDate],126), 1, (len(CONVERT(varchar,m.[createdDate],126)) - 4)) as ModifiedDate
      ,'Y' as [IsReplied]
      ,[IsVoice]
      ,[Subject],ayatollah,u2.UserType,p1.Name as FromName,p1.ImageID as FromImageID,
p2.Name as ToName,p2.ImageID as ToImageID,m.ContentType,m.FileTitle,m.FileSize,m.FileThumbnailID from messages m 
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
where replytoid=@id and u2.userLoginstatus='ONLINE'
END

GO
/****** Object:  StoredProcedure [dbo].[MaintainUserLoginStatus]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[MaintainUserLoginStatus]
(
@userid int,
@status varchar(10)
)
as
begin
if (@status ='OFFLINE')
begin
update Users set 
userLoginStatus =@status ,RegistrationID=null,
lastOnlineTime=getdate()
where userID=@userid;
end
else
begin
update Users set 
userLoginStatus =@status ,
lastOnlineTime=getdate()
where userID=@userid;
end

end

GO
/****** Object:  StoredProcedure [dbo].[MessageCountByUsername]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[MessageCountByUsername]
(
@username varchar(500),
@indc varchar(4)
)
as
begin

if @indc='IN'
begin
select count(*) as MessageCount,IsRead,'IN' as Direction from messages where messages.Status='APPROVE'
and SendTo=@username group by IsRead
end
if @indc='OUT'
begin

select count(*) as MessageCount,IsRead,'OUT' as Direction from messages where SentFrom=@username group by IsRead

end

if @indc='BOTH'
begin

select count(*) as MessageCount,IsRead,'IN' as Direction from messages where messages.Status='APPROVE'
and SendTo=@username group by IsRead
union
select count(*) as MessageCount,IsRead,'OUT' as Direction from messages where SentFrom=@username group by IsRead

end

end

GO
/****** Object:  StoredProcedure [dbo].[RedirectMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[RedirectMessage]
(
@messageId bigint,
@toId bigint,
@byId bigint,
@remarks nvarchar(500)
)
as
begin

declare @ToUserName varchar(100);
select @ToUserName=username from users where userID=@toId;

Update Messages set 
Status='APPROVE',
Remarks =@remarks,
SendTo=@ToUserName,
ModifiedBy=@byId,
ModifiedDate=GETDATE(),
IsRead='N'
where ID=@messageId;

end
GO
/****** Object:  StoredProcedure [dbo].[RegisterUser]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[RegisterUser]  
(  
@username varchar(100),  
@password varchar(100),  
@name nvarchar(500),  
@location varchar(200),  
@mobileNumber varchar(20)=null,  
@nationality varchar(200),  
@userType varchar(30),  
@userStatus varchar(10),  
@details varchar(max),  
@age decimal(10,2),  
@gender varchar(20),
@Language varchar(50),
@SpecialisationIn nvarchar(max),
@StudiesAt nvarchar(max)
)  
as  
begin  
  
SET NoCount on;  
declare @passphrase varchar(50) =newID();  
DECLARE @id int  
DECLARE @table table (id int)  
  
INSERT INTO users  
           (username  
           ,userType  
           ,password  
           ,passphrase  
           ,userStatus  
           ,createdBy  
           ,modifiedBy  
           ,createdDate  
           ,modifiedDate  
           ,lastLoginTimeStamp)  
           OUTPUT inserted.UserID into @table  
     VALUES  
     (@username,@userType,dbo.EncryptString(@passphrase,@password),@passphrase,  
     @userStatus,0,0,getdate(),getdate(),getdate());  
       
     SELECT @id = id from @table  
  
INSERT INTO userProfile  
           (userID  
           ,name  
           ,mobileNumber  
           ,location  
           ,nationality  
           ,createdBy  
           ,modifiedBy  
           ,createdDate  
           ,modifedDate  
           ,imageID  
           ,details  
           ,Age  
           ,Gender
           ,Language
           ,SpecialisationIn
           ,StudiesAt)  
     VALUES  
     (@id,@name,@mobileNumber,@location,@nationality,@id,0,  
     getdate(),getdate(),0,@details,@age,@gender,@Language,@SpecialisationIn,@StudiesAt);  
       
       
     select * from users where userID =@id  
  
end

GO
/****** Object:  StoredProcedure [dbo].[RegisterUserWithImage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[RegisterUserWithImage]  
(  
@username varchar(100),  
@password varchar(100),  
@name varchar(500),  
@location varchar(200),  
@mobileNumber varchar(20),  
@nationality varchar(200),  
@userType varchar(30),  
@userStatus varchar(10),  
@details varchar(max),  
@age decimal(10,2),  
@gender varchar(20),  
@FileName varchar(max),  
@ContentType varchar(100),  
@Data varbinary(max),
@Language varchar(50),
@SpecialisationIn nvarchar(max),
@StudiesAt nvarchar(max)  
)  
as  
begin  
  
SET NoCount on;  
declare @passphrase varchar(50) =newID();  
DECLARE @id int  
DECLARE @tblFileID int;  
DECLARE @table table (id int)  
DECLARE @tblFileIDTable table (id int)  
  
  
  
INSERT INTO tblFiles  
           (FileName  
           ,ContentType  
           ,Data)  
            OUTPUT inserted.ID into @tblFileIDTable  
     VALUES  
           (@FileName  
           ,@ContentType  
           ,@Data)  
  
  SELECT @tblFileID = id from @tblFileIDTable;  
  
INSERT INTO users  
           (username  
           ,userType  
           ,password  
           ,passphrase  
           ,userStatus  
           ,createdBy  
           ,modifiedBy  
           ,createdDate  
           ,modifiedDate  
           ,lastLoginTimeStamp)  
           OUTPUT inserted.UserID into @table  
     VALUES  
     (@username,@userType,dbo.EncryptString(@passphrase,@password),@passphrase,  
     @userStatus,0,0,getdate(),getdate(),getdate());  
       
     SELECT @id = id from @table  
  
INSERT INTO userProfile  
           (userID  
           ,name  
           ,mobileNumber  
           ,location  
           ,nationality  
           ,createdBy  
           ,modifiedBy  
           ,createdDate  
           ,modifedDate  
           ,details  
           ,Age  
           ,Gender  
           ,ImageID
           ,Language
           ,SpecialisationIn
           ,StudiesAt)  
     VALUES  
     (@id,@name,@mobileNumber,@location,@nationality,@id,0,  
     getdate(),getdate(),@details,@age,@gender,@tblFileID,@Language,@SpecialisationIn,@StudiesAt);  
       
       
     select * from users where userID =@id  
  
end

GO
/****** Object:  StoredProcedure [dbo].[ResendNotification]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ResendNotification]
as
begin

select u2.RegistrationID,u2.userType,
p1.Name as FromName,p1.ImageID as FromImageID,p2.Name as ToName,p2.ImageID as ToImageID,
m.* from Messages m
join users u1 on u1.username=m.SentFrom
join users u2 on u2.username=m.SendTo and u2.userType ='ALIM' and u2.userStatus='ACTV' and u2.userLoginStatus ='ONLINE'
join userProfile p1 on p1.userID=u1.userID
join userProfile p2 on p2.userID=u2.userID
and m.Status='APPROVE' and m.IsRead='N'

end
GO
/****** Object:  StoredProcedure [dbo].[ResendNotification2]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[ResendNotification2]
as
begin
select distinct u2.RegistrationID,p2.Name from Messages m 
join users u2 on u2.username= m.SendTo 
join userProfile p2 on p2.userID=u2.userID
where m.Status ='APPROVE' and m.IsRead='N'
and u2.userStatus='ACTV' and u2.userLoginStatus ='ONLINE'
end
GO
/****** Object:  StoredProcedure [dbo].[ScreenNameCount]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create Procedure [dbo].[ScreenNameCount]
(
@name varchar(500),
@userID int
)
as
begin
set @name=RTrim(LTrim(upper(@name)));
if(@userID=0)
	begin
	select COUNT(*) from userProfile where RTrim(LTrim(upper(name))) =@name;
	end
else
begin
select COUNT(*) from userProfile where RTrim(LTrim(upper(name))) =@name and userID !=@userID
end
end
GO
/****** Object:  StoredProcedure [dbo].[SendCommonMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[SendCommonMessage]
(
@Text nvarchar(max),
@SentFrom varchar(200),
@Status varchar(10),
@IsNotificationSent varchar(10),
@createdBy int,
@modifiedBy int
)
as
begin

INSERT INTO dbo.CommonMessages
           (Text
           ,SentFrom
           ,Status
           ,IsNotificationSent
           ,createdBy
           ,modifiedBy
           ,createdDate
           ,modifiedDate)
     VALUES
	 (
	 @Text,
	 @SentFrom,
	 @Status,
	 @IsNotificationSent,
	 @createdBy,
	 @modifiedBy,
	 GETDATE(),
	 GETDATE()
	 )
end
GO
/****** Object:  StoredProcedure [dbo].[StoreFile]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[StoreFile]
(
@FileName varchar(500),
@ContentType varchar(50),
@Data varbinary(max)
)
as
begin

INSERT INTO tblFiles
           (FileName
           ,ContentType
           ,Data)
     VALUES
           (@FileName
           ,@ContentType
           ,@Data)

end

GO
/****** Object:  StoredProcedure [dbo].[UpdateMessage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateMessage]
(
@ID int,
@Status varchar(10),
@userID int
)
as
begin

update Messages
set
Status=@Status,
ModifiedBy=@userID,
ModifiedDate=getdate()
where ID=@ID

end

GO
/****** Object:  StoredProcedure [dbo].[UpdateMultipleRead]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateMultipleRead]
(
@ID varchar(max),
@IsRead char(1),
@userID int
)
as
begin

update Messages
set
IsRead=@IsRead,
ModifiedBy=@userID,
ModifiedDate=getdate()
where ID in (SELECT Result FROM [dbo].[CSVtoTable](@ID,','))

end

GO
/****** Object:  StoredProcedure [dbo].[UpdateMultipleReply]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateMultipleReply]  
(  
@ID varchar(max),  
@IsReplied char(1),  
@userID int  
)  
as  
begin  
  
update Messages  
set  
IsReplied=@IsReplied,  
ModifiedBy=@userID,  
ModifiedDate=getdate()  
where ID in (SELECT Result FROM [dbo].[CSVtoTable](@ID,','))  
  
end

GO
/****** Object:  StoredProcedure [dbo].[UpdateProfile]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[UpdateProfile]  
(  
@userid int,  
@name nvarchar(500),  
@location varchar(200),  
@mobileNumber varchar(20)=null,  
@nationality varchar(200),  
@details varchar(max),  
@age decimal(5,2),  
@gender varchar(20),
@Language varchar(50),
@SpecialisationIn nvarchar(max),
@StudiesAt nvarchar(max)   ,
@archiveUrl nvarchar(2000)
)  
as  
begin  
  
  
UPDATE userProfile  
           Set name=@name  
           , mobileNumber=@mobileNumber  
           , location=@location  
           , nationality=@nationality  
           , modifiedBy=@userid  
           , modifedDate=getdate()  
           ,details=@details  
           ,age=@age  
           ,Gender=@gender 
            ,Language=@Language
		 	,SpecialisationIn=@SpecialisationIn
			,StudiesAt=@StudiesAt
			,archiveUrl=@archiveUrl
    where userID=@userid;  
  
end
GO
/****** Object:  StoredProcedure [dbo].[UpdateProfileImage]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateProfileImage]
(
@userid int,
@FileName varchar(max),
@ContentType varchar(100),
@Data varbinary(max)
)
as
begin

DECLARE @tblFileID int;
DECLARE @tblFileIDTable table (id int);
select @tblFileID=imageID from userprofile where userID =@userid;

if @tblFileID is not null and @tblFileID !=0
begin

delete from tblFiles where ID= @tblFileID;

end

INSERT INTO tblFiles
           (FileName
           ,ContentType
           ,Data)
            OUTPUT inserted.ID into @tblFileIDTable
     VALUES
           (@FileName
           ,@ContentType
           ,@Data);
 
   SELECT @tblFileID = id from @tblFileIDTable;
           
update userprofile set ImageID=@tblFileID,modifedDate=getdate()
where userID=@userid;



end

GO
/****** Object:  StoredProcedure [dbo].[UpdateRead]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateRead]  
(  
@ID int,  
@IsRead char(1),  
@userID int  
)  
as  
begin  
 
if not exists (select * from Messages where IsRead=@IsRead and ID=@ID)
begin
	update Messages  set  
	IsRead=@IsRead,  
	ModifiedBy=@userID,  
	ModifiedDate=getdate()  
	where ID=@ID  
end	 
end

GO
/****** Object:  StoredProcedure [dbo].[UpdateRegistrationID]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[UpdateRegistrationID]
(
@userId int,
@registrationID varchar(500)
)
as
begin

update Users set RegistrationID=null where 
RegistrationID=@registrationID;

update Users
set
RegistrationID=@registrationID
where userID=@userId;

end

GO
/****** Object:  StoredProcedure [dbo].[UpdateTypicallyRepliesIn]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Procedure [dbo].[UpdateTypicallyRepliesIn]
(
@userid int,
@TypicallyRepliesIn nvarchar(50)  = ''
)
as
begin

update userProfile set TypicallyRepliesIn=@TypicallyRepliesIn
where userID=@userid;
end
GO
/****** Object:  StoredProcedure [dbo].[updateUserStatus]    Script Date: 2/20/2018 11:57:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[updateUserStatus]
(
@userID int,
@status varchar(10)
)
as
begin
update users set userStatus=@status where userID =@userID;
end

GO
USE [master]
GO
ALTER DATABASE [ATWKAppDbFinal_TEST] SET  READ_WRITE 
GO
