USE eec_ict;
-- =============================================
-- Stored Procedures cho Dashboard giám sát hệ thống (MySQL)
-- =============================================

-- =============================================
-- Lấy thông tin tổng quan của hệ thống
-- =============================================
DELIMITER //
DROP PROCEDURE IF EXISTS GetSystemSummary //
CREATE PROCEDURE GetSystemSummary()
BEGIN
    -- Trả về kết quả
    SELECT 
        (SELECT COUNT(*) FROM Users WHERE Is_Admin = 0 AND Is_Active = 1) AS TotalUsers,
        (SELECT COUNT(*) FROM Exams) AS TotalExams,
        (SELECT COUNT(*) FROM Questions) AS TotalQuestions,
        (SELECT COUNT(*) FROM UserExamAttempts) AS TotalAttempts,
        (SELECT 
            CASE 
                WHEN COUNT(*) > 0 THEN (SUM(CASE WHEN Passed = 1 THEN 1 ELSE 0 END) / COUNT(*)) * 100
                ELSE 0
            END
         FROM UserExamAttempts
         WHERE End_Time IS NOT NULL) AS OverallPassRate;
END //
DELIMITER ;

-- =============================================
-- Lấy số lượng attempts theo thời gian
-- =============================================
DELIMITER //
DROP PROCEDURE IF EXISTS GetExamAttemptsOverTime //
CREATE PROCEDURE GetExamAttemptsOverTime(
    IN dFromDate DATETIME,
    IN dToDate DATETIME
)
BEGIN
    -- Nếu không có ngày bắt đầu, lấy 30 ngày gần nhất
    IF dFromDate IS NULL THEN
        SET dFromDate = DATE_SUB(CURDATE(), INTERVAL 30 DAY);
    END IF;
    
    -- Nếu không có ngày kết thúc, lấy đến ngày hiện tại
    IF dToDate IS NULL THEN
        SET dToDate = CURDATE();
    END IF;
    
    -- Tạo bảng tạm chứa tất cả các ngày trong khoảng thời gian
    DROP TEMPORARY TABLE IF EXISTS temp_dates;
    CREATE TEMPORARY TABLE temp_dates (Date DATE);
    
    SET @CurrentDate = dFromDate;
    
    -- Thêm tất cả các ngày vào bảng tạm
    WHILE @CurrentDate <= dToDate DO
        INSERT INTO temp_dates (Date) VALUES (DATE(@CurrentDate));
        SET @CurrentDate = DATE_ADD(@CurrentDate, INTERVAL 1 DAY);
    END WHILE;
    
    -- Lấy kết quả
    SELECT 
        d.Date,
        COUNT(a.Id) AS TotalAttempts,
        SUM(CASE WHEN a.Passed = 1 THEN 1 ELSE 0 END) AS PassedAttempts
    FROM temp_dates d
    LEFT JOIN UserExamAttempts a ON DATE(a.Start_Time) = d.Date
    GROUP BY d.Date
    ORDER BY d.Date;
    
    -- Xóa bảng tạm
    DROP TEMPORARY TABLE IF EXISTS temp_dates;
END //
DELIMITER ;

-- =============================================
-- Lấy phân bố điểm của các lần làm bài thi
-- =============================================
DELIMITER //
DROP PROCEDURE IF EXISTS GetScoreDistribution //
CREATE PROCEDURE GetScoreDistribution(
    IN iExamId INT
)
BEGIN
    -- Tạo bảng tạm chứa các khoảng điểm
    DROP TEMPORARY TABLE IF EXISTS temp_score_ranges;
    CREATE TEMPORARY TABLE temp_score_ranges (
        RangeId INT,
        RangeStart DECIMAL(5,2),
        RangeEnd DECIMAL(5,2),
        RangeLabel VARCHAR(20)
    );
    
    -- Thêm các khoảng điểm vào bảng tạm (0-2, 2-4, 4-6, 6-8, 8-10)
    INSERT INTO temp_score_ranges (RangeId, RangeStart, RangeEnd, RangeLabel)
    VALUES 
        (1, 0, 2, '0-2'),
        (2, 2.01, 4, '2-4'),
        (3, 4.01, 6, '4-6'),
        (4, 6.01, 8, '6-8'),
        (5, 8.01, 10, '8-10');
    
    -- Lấy kết quả
    SELECT 
        r.RangeLabel AS ScoreRange,
        COUNT(a.Id) AS Count
    FROM temp_score_ranges r
    LEFT JOIN UserExamAttempts a ON 
        ROUND(a.Total_Score, 2) >= r.RangeStart AND 
        ROUND(a.Total_Score, 2) <= r.RangeEnd AND
        (iExamId IS NULL OR a.Exam_Id = iExamId) AND
        a.End_Time IS NOT NULL
    GROUP BY r.RangeId, r.RangeLabel
    ORDER BY r.RangeId;
    
    -- Xóa bảng tạm
    DROP TEMPORARY TABLE IF EXISTS temp_score_ranges;
END //
DELIMITER ;

-- =============================================
-- Lấy danh sách các bài thi có tỷ lệ đậu cao nhất
-- =============================================
DELIMITER //
DROP PROCEDURE IF EXISTS GetTopExamsByPassRate //
CREATE PROCEDURE GetTopExamsByPassRate(
    IN iLimit INT
)
BEGIN
    IF iLimit IS NULL THEN
        SET iLimit = 10;
    END IF;
    
    SELECT 
        e.Id AS ExamId,
        e.Title AS ExamTitle,
        COUNT(a.Id) AS TotalAttempts,
        SUM(CASE WHEN a.Passed = 1 THEN 1 ELSE 0 END) AS PassedAttempts,
        CASE 
            WHEN COUNT(a.Id) > 0 THEN (SUM(CASE WHEN a.Passed = 1 THEN 1 ELSE 0 END) / COUNT(a.Id)) * 100
            ELSE 0
        END AS PassRate
    FROM Exams e
    LEFT JOIN UserExamAttempts a ON e.Id = a.Exam_Id AND a.End_Time IS NOT NULL
    GROUP BY e.Id, e.Title
    HAVING COUNT(a.Id) > 0
    ORDER BY PassRate DESC
    LIMIT iLimit;
END //
DELIMITER ;

-- =============================================
-- Lấy các hoạt động gần đây trong hệ thống
-- =============================================
DELIMITER //
DROP PROCEDURE IF EXISTS GetRecentActivities //
CREATE PROCEDURE GetRecentActivities(
    IN iLimit INT
)
BEGIN
    IF iLimit IS NULL THEN
        SET iLimit = 20;
    END IF;
    
    -- Tạo bảng tạm để lưu các hoạt động
    DROP TEMPORARY TABLE IF EXISTS temp_activities;
    CREATE TEMPORARY TABLE temp_activities (
        Id INT AUTO_INCREMENT PRIMARY KEY,
        ActivityType VARCHAR(50),
        UserId INT,
        UserName VARCHAR(100),
        Description VARCHAR(255),
        Timestamp DATETIME,
        RelatedEntityId INT NULL,
        RelatedEntityName VARCHAR(255) NULL
    );
    
    -- Thêm các lần làm bài thi gần đây
    INSERT INTO temp_activities (ActivityType, UserId, UserName, Description, Timestamp, RelatedEntityId, RelatedEntityName)
    SELECT 
        'exam_attempt' AS ActivityType,
        a.User_Id AS UserId,
        u.Username AS UserName,
        CASE 
            WHEN a.Passed = 1 THEN CONCAT('Đã hoàn thành bài thi "', e.Title, '" với điểm ', ROUND(a.Total_Score, 2), '/10 và đạt điểm đậu')
            WHEN a.Passed = 0 AND a.End_Time IS NOT NULL THEN CONCAT('Đã hoàn thành bài thi "', e.Title, '" với điểm ', ROUND(a.Total_Score, 2), '/10 nhưng không đạt điểm đậu')
            ELSE CONCAT('Đang làm bài thi "', e.Title, '"')
        END AS Description,
        a.Start_Time AS Timestamp,
        a.Exam_Id AS RelatedEntityId,
        e.Title AS RelatedEntityName
    FROM UserExamAttempts a
    INNER JOIN Users u ON a.User_Id = u.Id
    INNER JOIN Exams e ON a.Exam_Id = e.Id
    ORDER BY a.Start_Time DESC
    LIMIT iLimit;
    
    -- Thêm các đăng ký người dùng mới gần đây
    INSERT INTO temp_activities (ActivityType, UserId, UserName, Description, Timestamp, RelatedEntityId, RelatedEntityName)
    SELECT 
        'user_registration' AS ActivityType,
        u.Id AS UserId,
        u.Username AS UserName,
        'Đã đăng ký tài khoản mới' AS Description,
        u.Created_At AS Timestamp,
        NULL AS RelatedEntityId,
        NULL AS RelatedEntityName
    FROM Users u
    WHERE u.Is_Admin = 0
    ORDER BY u.Created_At DESC
    LIMIT iLimit;
    
    -- Lấy iLimit hoạt động gần nhất từ bảng tạm
    SELECT 
        Id,
        ActivityType,
        UserId,
        UserName,
        Description,
        Timestamp,
        RelatedEntityId,
        RelatedEntityName
    FROM temp_activities
    ORDER BY Timestamp DESC
    LIMIT iLimit;
    
    -- Xóa bảng tạm
    DROP TEMPORARY TABLE IF EXISTS temp_activities;
END //
DELIMITER ; 