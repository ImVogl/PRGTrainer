USE [master]
GO
/****** Object:  Database [PrgTrainerDb]    Script Date: 02.12.2019 1:48:51 ******/
CREATE DATABASE [PrgTrainerDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UserResults', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER14\MSSQL\DATA\UserResults.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'UserResults_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER14\MSSQL\DATA\UserResults_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [PrgTrainerDb] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PrgTrainerDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PrgTrainerDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PrgTrainerDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PrgTrainerDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PrgTrainerDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PrgTrainerDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET RECOVERY FULL 
GO
ALTER DATABASE [PrgTrainerDb] SET  MULTI_USER 
GO
ALTER DATABASE [PrgTrainerDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PrgTrainerDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PrgTrainerDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PrgTrainerDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PrgTrainerDb] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'PrgTrainerDb', N'ON'
GO
ALTER DATABASE [PrgTrainerDb] SET QUERY_STORE = OFF
GO
USE [PrgTrainerDb]
GO
/****** Object:  User [PRGTrainerUser]    Script Date: 02.12.2019 1:48:51 ******/
CREATE USER [PRGTrainerUser] FOR LOGIN [PRGTrainerUser] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [PRGTrainerAdmin]    Script Date: 02.12.2019 1:48:51 ******/
CREATE USER [PRGTrainerAdmin] FOR LOGIN [PRGTrainerAdmin] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 02.12.2019 1:48:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[UserId] [int] NOT NULL,
	[UserStat] [bit] NOT NULL,
	[QuestionsStat] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionResults]    Script Date: 02.12.2019 1:48:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionResults](
	[question] [varchar](400) NOT NULL,
	[wrongcount] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tokens]    Script Date: 02.12.2019 1:48:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tokens](
	[Token] [varchar](50) NOT NULL,
	[UserStat] [bit] NOT NULL,
	[QuestionsStat] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserResults]    Script Date: 02.12.2019 1:48:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserResults](
	[identifier] [int] NOT NULL,
	[username] [varchar](50) NULL,
	[result] [float] NOT NULL,
	[finishtime] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[AddAdmin]    Script Date: 02.12.2019 1:48:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddAdmin]
	@Token varchar(50) = NULL,
	@Identifier int = NULL
	
AS
BEGIN
	IF @Token IS NULL OR @Identifier IS NULL
	BEGIN
		RETURN
	END
	
	IF NOT EXISTS(SELECT * FROM dbo.Tokens WHERE Token = @Token)
	BEGIN
		RETURN
	END

	DECLARE @UserStat bit;
    DECLARE @QuestionsStat bit;
	
	SELECT @UserStat = UserStat FROM dbo.Tokens WHERE Token = @Token;
	SELECT @QuestionsStat = QuestionsStat FROM dbo.Tokens WHERE Token = @Token;
	
	INSERT INTO dbo.Admins (UserId, UserStat, QuestionsStat) VALUES(@Identifier, @UserStat, @QuestionsStat);
	DELETE dbo.Tokens WHERE Token = @Token;
END
GO
USE [master]
GO
ALTER DATABASE [PrgTrainerDb] SET  READ_WRITE 
GO
