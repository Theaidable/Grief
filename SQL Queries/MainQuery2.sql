-- Drop tables in reverse dependency order
DROP TABLE IF EXISTS Inventory;
DROP TABLE IF EXISTS Quest;
DROP TABLE IF EXISTS Enemy;
DROP TABLE IF EXISTS NPC;
DROP TABLE IF EXISTS Player;
DROP TABLE IF EXISTS Item;
DROP TABLE IF EXISTS Level;

-- Level Table
CREATE TABLE [dbo].[Level] (
    [levelID]   INT          IDENTITY (1, 1) NOT NULL,
    [levelName] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([levelID] ASC)
);


-- Player Table (playerID = saveID, PK)
CREATE TABLE [dbo].[Player] (
    [playerID]       INT        NOT NULL,
    [playerHealth]   INT        NOT NULL,
    [positionX]      FLOAT (53) NOT NULL,
    [positionY]      FLOAT (53) NOT NULL,
    [currentLevelID] INT        NULL,
    [saveTimeStamp]  DATETIME   NOT NULL,
    PRIMARY KEY CLUSTERED ([playerID] ASC),
    FOREIGN KEY ([currentLevelID]) REFERENCES [dbo].[Level] ([levelID])
);

-- NPC Table
CREATE TABLE [dbo].[NPC] (
    [npcID]                      INT            IDENTITY (1, 1) NOT NULL,
    [npcName]                    VARCHAR (50)   NULL,
    [positionX]                  FLOAT (53)     NOT NULL,
    [positionY]                  FLOAT (53)     NOT NULL,
    [levelID]                    INT            NULL,
    [dialogBeforeAccept]         NVARCHAR (MAX) NULL,
    [dialogAcceptedNotCompleted] NVARCHAR (MAX) NULL,
    [dialogOnCompleted]          NVARCHAR (MAX) NULL,
    [dialogAlreadyCompleted]     NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([npcID] ASC),
    FOREIGN KEY ([levelID]) REFERENCES [dbo].[Level] ([levelID])
);

-- Enemy Table
CREATE TABLE [dbo].[Enemy] (
    [enemyID]     INT           IDENTITY (1, 1) NOT NULL,
    [enemyType]   VARCHAR (100) NOT NULL,
    [enemyHealth] INT           NOT NULL,
    [positionX]   FLOAT (53)    NOT NULL,
    [positionY]   FLOAT (53)    NOT NULL,
    [levelID]     INT           NULL,
    PRIMARY KEY CLUSTERED ([enemyID] ASC),
    FOREIGN KEY ([levelID]) REFERENCES [dbo].[Level] ([levelID])
);

-- Item Table
CREATE TABLE [dbo].[Item] (
    [itemID]   INT           IDENTITY (1, 1) NOT NULL,
    [itemName] VARCHAR (100) NOT NULL,
    [itemType] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([itemID] ASC)
);

-- Quest Table
CREATE TABLE [dbo].[Quest] (
    [questID]          INT           IDENTITY (1, 1) NOT NULL,
    [questName]        VARCHAR (500) NOT NULL,
    [questGiver]       INT           NULL,
    [description]      VARCHAR (500) NULL,
    [requiredItemName] INT           NULL,
    [isAccepted]       BIT           NOT NULL,
    [isCompleted]      BIT           NOT NULL,
    [rewardItemName]   INT           NULL,
    PRIMARY KEY CLUSTERED ([questID] ASC),
    FOREIGN KEY ([questGiver]) REFERENCES [dbo].[NPC] ([npcID]),
    FOREIGN KEY ([requiredItemName]) REFERENCES [dbo].[Item] ([itemID]),
    CONSTRAINT [FK_Quest_Item] FOREIGN KEY ([rewardItemName]) REFERENCES [dbo].[Item] ([itemID])
);

-- Inventory Table
CREATE TABLE [dbo].[Inventory] (
    [inventoryID] INT IDENTITY (1, 1) NOT NULL,
    [playerID]    INT NULL,
    [itemID]      INT NULL,
    PRIMARY KEY CLUSTERED ([inventoryID] ASC),
    FOREIGN KEY ([playerID]) REFERENCES [dbo].[Player] ([playerID]),
    FOREIGN KEY ([itemID]) REFERENCES [dbo].[Item] ([itemID])
);
