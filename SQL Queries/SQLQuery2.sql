-- Drop tables in reverse dependency order
DROP TABLE IF EXISTS Inventory;
DROP TABLE IF EXISTS Quest;
DROP TABLE IF EXISTS Enemy;
DROP TABLE IF EXISTS NPC;
DROP TABLE IF EXISTS Player;
DROP TABLE IF EXISTS Item;
DROP TABLE IF EXISTS Level;

-- Level Table
CREATE TABLE Level (
    levelID INT IDENTITY(1,1) PRIMARY KEY,
    levelName VARCHAR(50) NOT NULL
);

-- Player Table (playerID = saveID, PK)
CREATE TABLE Player (
    playerID INT PRIMARY KEY,
    playerHealth INT NOT NULL,
    positionX FLOAT NOT NULL,
    positionY FLOAT NOT NULL,
    currentLevelID INT REFERENCES Level(levelID),
    saveTimeStamp DATETIME NOT NULL
);

-- NPC Table
CREATE TABLE NPC (
    npcID INT IDENTITY(1,1) PRIMARY KEY,
    npcName VARCHAR(50),
    positionX FLOAT NOT NULL,
    positionY FLOAT NOT NULL,
    levelID INT REFERENCES Level(levelID)
);

-- Enemy Table
CREATE TABLE Enemy (
    enemyID INT IDENTITY(1,1) PRIMARY KEY,
    enemyType VARCHAR(100) NOT NULL,
    enemyHealth INT NOT NULL,
    positionX FLOAT NOT NULL,
    positionY FLOAT NOT NULL,
    levelID INT REFERENCES Level(levelID)
);

-- Item Table
CREATE TABLE Item (
    itemID INT IDENTITY(1,1) PRIMARY KEY,
    itemName VARCHAR(100) NOT NULL,
    itemType VARCHAR(100) NOT NULL
);

-- Quest Table
CREATE TABLE Quest (
    questID INT IDENTITY(1,1) PRIMARY KEY,
    questName VARCHAR(500) NOT NULL,
    questGiver INT REFERENCES NPC(npcID),
    description VARCHAR(500),
    requiredItemName INT REFERENCES Item(itemID),
    isAccepted INT NOT NULL,
    isCompleted INT NOT NULL
);

-- Inventory Table
CREATE TABLE Inventory (
    inventoryID INT IDENTITY(1,1) PRIMARY KEY,
    playerID INT REFERENCES Player(playerID),
    itemID INT REFERENCES Item(itemID)
);
