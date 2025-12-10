import { apiClient } from './apiClient';

// This interface matches the full EarningRule entity
export interface EarningRule {
    id: string;
    name: string;
    description?: string;
    active: boolean;
    priority: number;
    version: number;
    type: string;
    conditionJson?: string;
    pointsJson?: string;
    limitsJson?: string;
    timeWindowJson?: string;
    segmentsJson?: string;
    metadata?: string;
    createdAt: string;
    updatedAt: string;
}

// This interface matches the backend CreateEarningRuleDto
export interface CreateEarningRulePayload {
    name: string;
    description?: string;
    priority: number;
    active: boolean;
    condition: object;
    points: object;
    limits?: object;
    timeWindow?: object;
    segments?: string[];
    metadata?: object;
}

export const getEarningRules = async (): Promise<EarningRule[]> => {
    return apiClient.get<EarningRule[]>('/earningrules');
};

export const createEarningRule = async (payload: CreateEarningRulePayload): Promise<EarningRule> => {
  return apiClient.post('/earningrules', payload);
};
