export type NavItem = {
  title: string;
  url: string;
  icon?: string;
  isActive?: boolean;
  shortcut?: string[];
  items?: NavItem[];
};

export type Gender = 'Male' | 'Female' | 'Other';

// Represents the new, richer data for the member list
export interface MemberListItem {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    tierName?: string;
    pointsBalance: number;
    status: string;
    joinDate: string;
}

// The detailed customer model for the single member view page
export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  gender?: Gender;
  dateOfBirth?: string;
  registrationDate: string;
  hasAgreedToTerms: boolean;
  isActive: boolean;
  street?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  pointsBalance: number;
  loyaltyTierName: string;
  referralCode?: string;
  loyaltyCardNumber?: string;
}

export interface CreateMemberData {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber?: string;
    dateOfBirth?: string;
    gender?: Gender;
}

export interface Wallet {
  id: string;
  type: 'POINTS' | 'CASHBACK';
  balance: number;
  currency?: string;
  expiringSoon?: number;
  nextExpiryDate?: string;
}

export interface WalletTransaction {
  id: string;
  date: string;
  walletType: 'POINTS' | 'CASHBACK';
  direction: 'EARN' | 'SPEND' | 'EXPIRE' | 'ADJUST';
  amount: number;
  balanceAfter: number;
  reason: string;
  referenceType?: string;
  referenceId?: string;
  campaignId?: number;
}

export interface ExpiringPoint {
  expiryDate: string;
  points: number;
  source?: string;
}

export interface Tier {
    id: string;
    name: string;
    level: number;
    pointsThreshold: number;
    since?: string;
    nextTier?: string;
    progress?: {
        current: number;
        required: number;
    };
}

export interface TierHistoryItem {
    tierName: string;
    startDate: string;
    endDate?: string;
}

export interface Transaction {
    id: string;
    date: string;
    amount: number;
    currency: string;
    items: { name: string; quantity: number; }[];
}

export interface CustomEvent {
    id: string;
    name: string;
    date: string;
    data: Record<string, any>;
}

export interface Campaign {
    id: string;
    name: string;
    description: string;
}

export interface Achievement {
    id: string;
    name: string;
    description: string;
}

export interface MemberReward {
    id: string;
    rewardName: string;
    status: 'ISSUED' | 'REDEEMED' | 'EXPIRED';
    issueDate: string;
    expiryDate?: string;
}

export interface TimelineEvent {
  id: number;
  eventType: 'POINT_EARN' | 'POINT_SPEND' | 'POINT_EXPIRE' | 'CASHBACK_EARN' | 'CASHBACK_SPEND' | 'TIER_CHANGE' | 'PROFILE_UPDATE' | 'TAG_ADDED' | 'TAG_REMOVED' | 'REWARD_REDEEM' | 'CAMPAIGN_ENROLLED' | 'LOGIN_EVENT';
  title: string;
  description?: string;
  walletType?: 'POINTS' | 'CASHBACK';
  pointsChange?: number;
  cashbackChange?: number;
  balanceAfter?: number;
  oldValue?: string;
  newValue?: string;
  referenceType?: string;
  referenceId?: string;
  createdAt: string;
}
