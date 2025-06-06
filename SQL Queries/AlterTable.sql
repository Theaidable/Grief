ALTER TABLE Quest ADD rewardItemName INT NULL; -- Fremmednøgle til Item-tabellen

ALTER TABLE Quest
ADD CONSTRAINT FK_Quest_Item FOREIGN KEY (rewardItemName)
REFERENCES Item(itemID);

ALTER TABLE NPC 
ADD 
dialogBeforeAccept NVARCHAR(MAX),
dialogAcceptedNotCompleted NVARCHAR(MAX),
dialogOnCompleted NVARCHAR(MAX), 
dialogAlreadyCompleted NVARCHAR(MAX);