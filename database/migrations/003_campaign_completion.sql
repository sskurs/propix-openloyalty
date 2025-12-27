-- Phase 1: Core Data Infrastructure
-- Database: loyalty_db (Admin API)

-- 1. Member Points Balance Table
CREATE TABLE IF NOT EXISTS member_points (
    id BIGSERIAL PRIMARY KEY,
    member_id TEXT NOT NULL,
    points_balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    lifetime_points DECIMAL(18,2) NOT NULL DEFAULT 0,
    points_earned_this_month DECIMAL(18,2) NOT NULL DEFAULT 0,
    points_redeemed DECIMAL(18,2) NOT NULL DEFAULT 0,
    tier TEXT DEFAULT 'BRONZE',
    last_updated_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    CONSTRAINT unique_member_id UNIQUE(member_id)
);

CREATE INDEX IF NOT EXISTS idx_member_points_member_id ON member_points(member_id);

-- 2. Points Transaction History Table
CREATE TABLE IF NOT EXISTS points_transactions (
    id BIGSERIAL PRIMARY KEY,
    member_id TEXT NOT NULL,
    transaction_type TEXT NOT NULL, -- 'EARNED', 'REDEEMED', 'EXPIRED', 'ADJUSTED'
    points DECIMAL(18,2) NOT NULL,
    balance_after DECIMAL(18,2) NOT NULL,
    campaign_id TEXT,
    order_id TEXT,
    description TEXT,
    metadata JSONB,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_points_tx_member_id ON points_transactions(member_id);
CREATE INDEX IF NOT EXISTS idx_points_tx_created_at ON points_transactions(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_points_tx_campaign_id ON points_transactions(campaign_id);

-- 3. Campaign Enrollments Table
CREATE TABLE IF NOT EXISTS campaign_enrollments (
    id BIGSERIAL PRIMARY KEY,
    campaign_id TEXT NOT NULL,
    member_id TEXT NOT NULL,
    enrolled_at TIMESTAMP WITH TIME ZONE NOT NULL,
    status TEXT NOT NULL DEFAULT 'ACTIVE', -- 'ACTIVE', 'COMPLETED', 'CANCELLED'
    progress JSONB,
    rewards_earned JSONB,
    CONSTRAINT unique_campaign_member UNIQUE(campaign_id, member_id)
);

CREATE INDEX IF NOT EXISTS idx_enrollments_member_id ON campaign_enrollments(member_id);
CREATE INDEX IF NOT EXISTS idx_enrollments_campaign_id ON campaign_enrollments(campaign_id);
CREATE INDEX IF NOT EXISTS idx_enrollments_status ON campaign_enrollments(status);

-- Insert sample data for testing (optional)
-- This creates a default member_points record for existing users
INSERT INTO member_points (member_id, points_balance, lifetime_points, tier, last_updated_at, created_at)
SELECT DISTINCT user_id, 0, 0, 'BRONZE', NOW(), NOW()
FROM transaction_events
WHERE user_id NOT IN (SELECT member_id FROM member_points)
ON CONFLICT (member_id) DO NOTHING;
