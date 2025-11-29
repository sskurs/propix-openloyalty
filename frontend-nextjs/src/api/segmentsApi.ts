import { apiClient } from './apiClient';
import { MemberListItem } from '@/types';

export interface CreateSegmentPayload {
  name: string;
  description?: string;
  type: 'DYNAMIC' | 'STATIC';
  conditions: object; // The rule builder's state
}

export interface Segment {
    id: string;
    name: string;
    description?: string;
    type: string;
    conditionsJson: string;
    status: string;
    createdAt: string;
}

export const createSegment = async (payload: CreateSegmentPayload): Promise<any> => {
  return apiClient.post('/segments', payload);
};

export const getSegments = async (): Promise<Segment[]> => {
    return apiClient.get<Segment[]>('/segments');
};

export const getSegmentById = async (id: string): Promise<Segment> => {
    return apiClient.get<Segment>(`/segments/${id}`);
};

export const getSegmentMembers = async (id: string): Promise<MemberListItem[]> => {
    return apiClient.get<MemberListItem[]>(`/segments/${id}/members`);
};

export const updateSegment = async (id: string, payload: Partial<CreateSegmentPayload>): Promise<any> => {
    return apiClient.put(`/segments/${id}`, payload);
};

export const updateSegmentStatus = async (id: string, status: 'ACTIVE' | 'DRAFT'): Promise<any> => {
    return apiClient.patch(`/segments/${id}/status`, { status });
};
