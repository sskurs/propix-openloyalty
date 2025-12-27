START TRANSACTION;
ALTER TABLE member_coupon DROP CONSTRAINT "FK_member_coupon_coupon_CouponId";

ALTER TABLE member_coupon DROP CONSTRAINT "FK_member_coupon_member_MemberId";

ALTER TABLE member_coupon DROP CONSTRAINT "PK_member_coupon";

DROP INDEX "IX_member_coupon_CouponId";

DROP INDEX "IX_member_coupon_MemberId";

ALTER TABLE member_coupon DROP COLUMN "CouponId";

ALTER TABLE member_coupon DROP COLUMN "UsedCount";

ALTER TABLE member_coupon RENAME TO member_coupons;

ALTER TABLE member_coupons RENAME COLUMN "LastUsedAt" TO "RedeemedAt";

ALTER TABLE campaign_conditions ADD "RuleGroupJson" jsonb;

ALTER TABLE member_coupons ALTER COLUMN "MemberId" TYPE text;

ALTER TABLE member_coupons ADD "CouponCode" text NOT NULL DEFAULT '';

ALTER TABLE member_coupons ADD CONSTRAINT "PK_member_coupons" PRIMARY KEY ("Id");

CREATE TABLE campaign_executions (
    "Id" uuid NOT NULL,
    "RuleId" text NOT NULL,
    "TransactionId" text,
    "MemberId" uuid NOT NULL,
    "ExecutedAt" timestamp with time zone NOT NULL,
    "RewardType" text,
    "RewardValue" numeric,
    CONSTRAINT "PK_campaign_executions" PRIMARY KEY ("Id")
);

UPDATE member SET "ExternalId" = 'CUST-1001'
WHERE "Id" = '55555555-5555-5555-5555-555555555555';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251225071956_AddRuleGroupJsonToCampaignConditions', '9.0.0');

COMMIT;

