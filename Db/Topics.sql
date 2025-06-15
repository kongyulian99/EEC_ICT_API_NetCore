-- Create Topics table
CREATE TABLE IF NOT EXISTS Topics (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(100) NOT NULL UNIQUE,
    `Description` TEXT,
    `Parent_Id` INT,
    `Created_At` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    `Updated_At` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    -- Định nghĩa Khóa ngoại tự tham chiếu
    FOREIGN KEY (`Parent_Id`) REFERENCES `Topics`(`Id`)
        ON DELETE SET NULL -- Khi topic cha bị xóa, Parent_Id của topic con sẽ được đặt là NULL (biến thành topic cấp 1)
        ON UPDATE CASCADE  -- Cập nhật Id của topic cha sẽ tự động cập nhật Parent_Id của topic con
);

USE eec_ict;

-- SQL File: topics_stored_procedures.sql

DELIMITER //

-- -----------------------------------------------------
-- Stored Procedures for Topics Management
-- -----------------------------------------------------

-- Drop existing procedures if they exist to avoid errors during re-creation
DROP PROCEDURE IF EXISTS `CreateTopic`//
DROP PROCEDURE IF EXISTS `UpdateTopic`//
DROP PROCEDURE IF EXISTS `DeleteTopic`//
DROP PROCEDURE IF EXISTS `GetTopicById`//
DROP PROCEDURE IF EXISTS `GetAllTopics`//
DROP PROCEDURE IF EXISTS `GetChildTopics`//


-- 1. Stored Procedure: CreateTopic
-- Tạo một topic mới, có thể là topic cha (Parent_Id = NULL) hoặc topic con (Parent_Id trỏ đến Id của topic cha).
CREATE PROCEDURE `CreateTopic`(
    IN sName VARCHAR(100),
    IN sDescription TEXT,
    IN iParent_Id INT -- NULL nếu là topic cha, Id của topic cha nếu là topic con
)
BEGIN
    INSERT INTO `Topics` (`Name`, `Description`, `Parent_Id`)
    VALUES (sName, sDescription, iParent_Id);

    SELECT LAST_INSERT_ID() AS New_Topic_Id;
END //

-- 2. Stored Procedure: UpdateTopic
-- Cập nhật thông tin của một topic.
CREATE PROCEDURE `UpdateTopic`(
    IN iId INT,
    IN sName VARCHAR(100),
    IN sDescription TEXT,
    IN iParent_Id INT
)
BEGIN
    UPDATE `Topics`
    SET
        `Name` = sName,
        `Description` = sDescription,
        `Parent_Id` = iParent_Id,
        `Updated_At` = CURRENT_TIMESTAMP
    WHERE `Id` = iId;
END //

-- 3. Stored Procedure: DeleteTopic
-- Xóa một topic.
-- Lưu ý: ON DELETE SET NULL trên khóa ngoại Parent_Id trong bảng Topics
-- và ON DELETE RESTRICT trên Topic_Id trong bảng Questions sẽ xử lý mối quan hệ nếu bạn xóa topic cha hoặc topic đang có câu hỏi.
CREATE PROCEDURE `DeleteTopic`(
    IN iId INT
)
BEGIN
    DELETE FROM `Topics`
    WHERE `Id` = iId;
END //

-- 4. Stored Procedure: GetTopicById
-- Lấy thông tin của một topic theo ID.
CREATE PROCEDURE `GetTopicById`(
    IN iId INT
)
BEGIN
    SELECT
        `Id`,
        `Name`,
        `Description`,
        `Parent_Id`,
        `Created_At`,
        `Updated_At`
    FROM `Topics`
    WHERE `Id` = iId;
END //

-- 5. Stored Procedure: GetAllTopics
-- Lấy tất cả các topic (bao gồm cả topic cha và con).
-- Sắp xếp theo Parent_Id và Name để dễ nhìn cấu trúc cây.
CREATE PROCEDURE `GetAllTopics`()
BEGIN
    SELECT
        `Id`,
        `Name`,
        `Description`,
        `Parent_Id`,
        `Created_At`,
        `Updated_At`
    FROM `Topics`
    ORDER BY `Parent_Id` ASC, `Name` ASC;
END //

-- 6. Stored Procedure: GetChildTopics
-- Lấy tất cả các topic con trực tiếp của một topic cha cụ thể.
CREATE PROCEDURE `GetChildTopics`(
    IN iParent_Id INT
)
BEGIN
    SELECT
        `Id`,
        `Name`,
        `Description`,
        `Parent_Id`,
        `Created_At`,
        `Updated_At`
    FROM `Topics`
    WHERE `Parent_Id` = iParent_Id
    ORDER BY `Name` ASC;
END //

DELIMITER ;