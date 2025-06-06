--<author>Asbjørn Ryberg</author>

-- Slet alt fra afhængige tabeller først (pga. foreign keys)
DELETE FROM Inventory;
DELETE FROM Quest;
DELETE FROM Enemy;
DELETE FROM NPC;

-- Til sidst: Slet alle Player (save slots)
DELETE FROM Player;
