import { MemberListItem, Customer as MemberDetails, Wallet, CreateMemberData, TimelineEvent, ExpiringPoint } from '@/types';
import { apiClient } from './apiClient';

// Mapper functions to ensure data consistency between API and frontend types
const mapToMemberListItem = (data: any): MemberListItem => ({
    id: data.id,
    firstName: data.firstName,
    lastName: data.lastName,
    email: data.email,
    phone: data.phone,
    tierName: data.tierName,
    pointsBalance: data.pointsBalance,
    status: data.status,
    joinDate: data.joinDate,
});

const mapToMemberDetails = (data: any): MemberDetails => ({
    // Assuming the detailed view DTO from backend matches this structure
    ...data
});

const mapToWallet = (data: any): Wallet => ({
    id: data.id,
    type: data.type,
    balance: data.balance,
    currency: data.currency,
});

const mapToTimelineEvent = (data: any): TimelineEvent => ({
    // Assuming the timeline event DTO from backend matches this structure
    ...data
});

const mapToExpiringPoint = (data: any): ExpiringPoint => ({
    // Assuming the expiring point DTO from backend matches this structure
    ...data
});


// API service functions

export const getMembers = async (): Promise<MemberListItem[]> => {
    const response = await apiClient.get<any[]>('/members');
    return Array.isArray(response) ? response.map(mapToMemberListItem) : [];
};

export const getMemberDetails = async (id: string): Promise<MemberDetails | null> => {
    const response = await apiClient.get<any>(`/members/${id}`);
    return response ? mapToMemberDetails(response) : null;
};

export const getMemberWallets = async (id: string): Promise<Wallet[]> => {
    const response = await apiClient.get<any[]>(`/members/${id}/wallets`);
    return Array.isArray(response) ? response.map(mapToWallet) : [];
};

export const getMemberTimeline = async (id: string): Promise<TimelineEvent[]> => {
    const response = await apiClient.get<any[]>(`/members/${id}/timeline`);
    return Array.isArray(response) ? response.map(mapToTimelineEvent) : [];
};

export const getMemberExpiringPoints = async (id: string): Promise<ExpiringPoint[]> => {
    const response = await apiClient.get<any[]>(`/members/${id}/expiring-points`);
    return Array.isArray(response) ? response.map(mapToExpiringPoint) : [];
};

export const createMember = async (memberData: CreateMemberData): Promise<any> => {
    return apiClient.post('/members', memberData);
};

export const toggleMemberStatus = async (id: string, isActive: boolean): Promise<void> => {
    await apiClient.patch(`/members/${id}/status`, { isActive });
};
