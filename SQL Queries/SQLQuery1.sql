--<author>Asbjørn Ryberg</author>

-- Player
CREATE TABLE Player (
    PlayerID INT PRIMARY KEY IDENTITY,
    PlayerName NVARCHAR(50) NOT NULL
);

-- Level
CREATE TABLE Level (
    LevelID INT PRIMARY KEY IDENTITY,
    LevelName NVARCHAR(50) NOT NULL
);

-- NPC
CREATE TABLE NPC (
    NPCID INT PRIMARY KEY IDENTITY,
    NPCName NVARCHAR(50) NOT NULL,
    PosX INT NOT NULL,
    PosY INT NOT NULL,
    LevelID INT NOT NULL,
    FOREIGN KEY (LevelID) REFERENCES Level(LevelID)
);

-- Quest
CREATE TABLE Quest (
    QuestID INT PRIMARY KEY IDENTITY,
    QuestName NVARCHAR(50) NOT NULL,
    QuestDescription NVARCHAR(100),
    CompletionStatus BIT NOT NULL,
    NPCID INT NOT NULL,
    FOREIGN KEY (NPCID) REFERENCES NPC(NPCID)
);

-- Enemies
CREATE TABLE Enemies (
    EnemyID INT PRIMARY KEY IDENTITY,
    EnemyName NVARCHAR(50) NOT NULL,
    EnemyHealth INT NOT NULL,
    PosX INT NOT NULL,
    PosY INT NOT NULL,
    LevelID INT NOT NULL,
    FOREIGN KEY (LevelID) REFERENCES Level(LevelID)
);

-- SaveGame
CREATE TABLE SaveGame (
    SaveID INT PRIMARY KEY IDENTITY,
    SaveName NVARCHAR(50) NOT NULL,
    PlayerHealth INT NOT NULL,
    PosX INT NOT NULL,
    PosY INT NOT NULL,
    CurrentLevelID INT NOT NULL,
    SaveTimeStamp DATETIME NOT NULL,
    PlayerID INT NOT NULL,
    FOREIGN KEY (CurrentLevelID) REFERENCES Level(LevelID),
    FOREIGN KEY (PlayerID) REFERENCES Player(PlayerID)
);

-- Item
CREATE TABLE Item (
    ItemID INT PRIMARY KEY IDENTITY,
    ItemName NVARCHAR(50) NOT NULL,
    ItemDescription NVARCHAR(100)
);

-- Inventory
CREATE TABLE Inventory (
    InventoryID INT PRIMARY KEY IDENTITY,
    SaveID INT NOT NULL,
    ItemID INT NOT NULL,
    FOREIGN KEY (SaveID) REFERENCES SaveGame(SaveID),
    FOREIGN KEY (ItemID) REFERENCES Item(ItemID)
);
