-- Migration: Add Progress Tracking to Jobs Table
-- Date: 2026-02-10
-- Description: Thêm các cột Progress, Status, LastUpdated vào Jobs table

-- Thêm các cột mới
ALTER TABLE Jobs
ADD COLUMN Progress INT DEFAULT 0 COMMENT 'Phần trăm hoàn thành (0-100)',
ADD COLUMN Status VARCHAR(20) DEFAULT 'Pending' COMMENT 'Trạng thái: Pending/In Progress/Completed',
ADD COLUMN LastUpdated DATETIME COMMENT 'Thời gian cập nhật tiến độ lần cuối';

-- Update existing records với giá trị mặc định
UPDATE Jobs 
SET Progress = 0, 
    Status = 'Pending', 
    LastUpdated = NOW()
WHERE Progress IS NULL;

-- Add check constraint để đảm bảo Progress trong khoảng 0-100
ALTER TABLE Jobs
ADD CONSTRAINT chk_progress_range CHECK (Progress >= 0 AND Progress <= 100);

-- Add index cho Status để query nhanh hơn
CREATE INDEX idx_jobs_status ON Jobs(Status);

-- Verify changes
SELECT 'Migration completed successfully!' AS Status;
DESCRIBE Jobs;
